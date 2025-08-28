using Bloodthirst.Scripts.Utils;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
using static Bloodthirst.Scripts.Utils.PathUtils;

public class ManhattenPathFollower : MonoBehaviour
{
    [SerializeField]
    private Transform[] pointsAsTransforms;

    [SerializeField]
    private Transform follower;

    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> manhattenPoints = new List<Vector3>();

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private WALKING_RESULT result;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private WalkingState state;

    [SerializeField]
    private bool isFollowing;

    [SerializeField]
    private float speed;

#if ODIN_INSPECTOR
    [Button]
#endif

    private void ResetFollow()
    {
        isFollowing = false;
        result = WALKING_RESULT.IDLE;

        RefreshState();
    }

    private void RefreshState()
    {
        state = new WalkingState();
        state.currentPosition = follower.position;
        state.pathIndex = 0;
        state.totalDistanceWalked = 0;
    }


#if ODIN_INSPECTOR
    [Button]
#endif

    private void StartFollow()
    {
        isFollowing = true;
        result = WALKING_RESULT.WALKING;

        RefreshState();
    }

    void Update()
    {
        if (!isFollowing)
            return;

        if (result == WALKING_RESULT.DONE)
            return;

        float delta = Time.deltaTime;

        result = PathUtils.WalkAlongPathManhatten(points, speed * delta, ref state);

        follower.position = state.currentPosition;
    }

    private void OnDrawGizmos()
    {
        points.Clear();
        manhattenPoints.Clear();

        for (int i = 0; i < pointsAsTransforms.Length; i++)
        {
            points.Add(pointsAsTransforms[i].position);
        }

        PathUtils.NormalToManhattenPathConverter(points, manhattenPoints);

        Gizmos.color = Color.red;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < manhattenPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(manhattenPoints[i], manhattenPoints[i + 1]);
        }
    }
}