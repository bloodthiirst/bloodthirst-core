using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothSim : MonoBehaviour
{
    public struct ClothPoint
    {
        public Vector2Int index;

        public Vector3 old_velocity;
        public Vector3 old_accleration;
        public Vector3 old_position;

        public Vector3 acceleration;
        public Vector3 velocity;
        public Vector3 position;

    }

    public float gizmosSize;
    public Color pointColor;
    public Color lineColor;

    public ClothPoint[] clothPoints;

    public float distance;

    /// <summary>
    /// Multiple of the normal distance comparerd to the max distance
    /// </summary>
    [Range(1f, 2f)]
    public float stretchFactor;

    public float mass = 1;

    public float damping;

    public float stiffness;

    public Vector3 gravityVector;

    public int width;
    public int height;
    public int size;

    public ClothPoint[] tempArray;

    private float initStiffness;

    [Button]
    private void InitCloth()
    {
        size = width * height;
        clothPoints = new ClothPoint[size];
        tempArray = new ClothPoint[size];
        initStiffness = stiffness;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int index = x + (width * y);
                ref ClothPoint point = ref clothPoints[index];
                point.index.x = x;
                point.index.y = y;
                point.position = transform.position + (distance * x * Vector3.right) + (distance * y * Vector3.down);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        bool spacePressed = Input.GetKeyDown(KeyCode.Space);

        if (clothPoints == null && spacePressed)
        {
            InitCloth();
            return;
        }

        if (clothPoints == null)
            return;

        for (int x = 0; x < width; ++x)
        {
            clothPoints[x].position = transform.position + (distance * x * Vector3.right) + (distance * 0 * Vector3.down);
        }

        Array.Copy(clothPoints, tempArray, clothPoints.Length);

        for (int x = 0; x < width; ++x)
        {
            for (int y = 1; y < height; ++y)
            {
                // 5 : 4 neighbors + 1 from gravity
                Vector3[] currToNeighborDistances = new Vector3[5];
                Vector3[] neighborVel = new Vector3[5];
                ref ClothPoint curr = ref Get(tempArray, x, y);
                int neightborCount = 0;

                if (x > 0)
                {
                    ref ClothPoint neighbor = ref Get(tempArray, x - 1, y);

                    Vector3 distanceVec = neighbor.position - curr.position;
                    currToNeighborDistances[neightborCount++] = distanceVec;

                    Vector3 velVec = neighbor.velocity;
                    neighborVel[neightborCount] = velVec;
                }

                if (x < width - 1)
                {
                    ref ClothPoint neighbor = ref Get(tempArray, x + 1, y);

                    Vector3 distanceVec = neighbor.position - curr.position;
                    currToNeighborDistances[neightborCount++] = distanceVec;

                    Vector3 velVec = neighbor.velocity;
                    neighborVel[neightborCount] = velVec;
                }

                if (y > 0)
                {
                    ref ClothPoint neighbor = ref Get(tempArray, x, y - 1);

                    Vector3 distanceVec = neighbor.position - curr.position;
                    currToNeighborDistances[neightborCount++] = distanceVec;

                    Vector3 velVec = neighbor.velocity;
                    neighborVel[neightborCount] = velVec;
                }

                if (y < height - 1)
                {
                    ref ClothPoint neighbor = ref Get(tempArray, x, y + 1);

                    Vector3 distanceVec = neighbor.position - curr.position;
                    currToNeighborDistances[neightborCount++] = distanceVec;

                    Vector3 velVec = neighbor.velocity;
                    neighborVel[neightborCount] = velVec;
                }

                Vector3 fSprings = Vector3.zero;


                for (int i = 0; i < neightborCount; ++i)
                {
                    // full formula :
                    // spring force : F = (-Ks(L0 - L) - Kd (v1 - v2)) * -dir

                    Vector3 PQVec = currToNeighborDistances[i];

                    float L = PQVec.magnitude;

                    // spring
                    float Ks = stiffness;
                    float Kd = damping;
                    float L0 = distance;
                    Vector3 dir = (PQVec / L);

                    // damp
                    float v2 = Vector3.Dot(dir, neighborVel[i]);
                    float v1 = Vector3.Dot(dir, curr.velocity);

                    fSprings += ((Ks * (L0 - L)) + (Kd * (v1 - v2))) * -dir;
                }


                // totalForce
                Vector3 FofT = Vector3.zero;
                FofT += fSprings;
                //FofT += gravityVector;

#if true
                // acceleration = F / mass
                curr.old_accleration = curr.acceleration;
                curr.acceleration = gravityVector + (FofT / mass);

                // todo: improve verlet integration

                curr.old_velocity = curr.velocity;
                curr.velocity = curr.old_velocity + (((curr.old_accleration + curr.acceleration) / 2) * dt);

                curr.old_position = curr.position;
                Vector3 newPos = curr.old_position + (curr.velocity * dt) + (0.5f * curr.acceleration * dt * dt);

                // set the point in the real array
                ref ClothPoint resultCurr = ref Get(clothPoints, x, y);
                resultCurr.old_accleration = curr.old_accleration;
                resultCurr.old_velocity = curr.old_velocity;
                resultCurr.old_position = curr.old_position;

                resultCurr.acceleration = curr.acceleration;
                resultCurr.velocity = curr.velocity;
                resultCurr.position = newPos;
#else
                    ref ClothPoint resultCurr = ref Get(clothPoints, x, y);
                    resultCurr.position += FofT * dt;
#endif

            }
        }

    }

    private ref ClothPoint Get(ClothPoint[] arr, int x, int y)
    {
        return ref arr[x + (width * y)];
    }

    private void OnDrawGizmos()
    {
        if (clothPoints == null)
            return;

        Gizmos.color = pointColor;

        for (int i = 0; i < size; ++i)
        {
            ClothPoint curr = clothPoints[i];
            Gizmos.DrawSphere(curr.position, gizmosSize);
        }

        Gizmos.color = lineColor;

        for (int y = 0; y < height - 1; ++y)
        {
            for (int x = 0; x < width - 1; ++x)
            {
                int currIndex = x + (width * y);
                int nextIndex = currIndex + 1;
                int bottomIndex = x + (width * (y + 1));
                ref ClothPoint currPoint = ref clothPoints[currIndex];
                ref ClothPoint nextPoint = ref clothPoints[nextIndex];
                ref ClothPoint bottomPoint = ref clothPoints[bottomIndex];

                Gizmos.DrawLine(currPoint.position, nextPoint.position);
                Gizmos.DrawLine(currPoint.position, bottomPoint.position);
            }

            {
                ref ClothPoint currPoint = ref Get(clothPoints, width - 1, y);
                ref ClothPoint bottomPoint = ref Get(clothPoints, width - 1, y + 1);
                Gizmos.DrawLine(currPoint.position, bottomPoint.position);
            }
        }

        {
            for (int x = 0; x < width - 1; ++x)
            {
                ref ClothPoint currPoint = ref Get(clothPoints, x, height - 1);
                ref ClothPoint nextPoint = ref Get(clothPoints, x + 1, height - 1);

                Gizmos.DrawLine(currPoint.position, nextPoint.position);
            }
        }
    }
}
