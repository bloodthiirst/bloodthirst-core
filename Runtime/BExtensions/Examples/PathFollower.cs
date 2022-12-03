using Bloodthirst.Scripts.Utils;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using static Bloodthirst.Scripts.Utils.PathUtils;

public class PathFollower : MonoBehaviour
{

#if ODIN_INSPECTOR
    [BoxGroup("Inputs")]
#endif
    [SerializeField]
    private double speed;

#if ODIN_INSPECTOR
    [BoxGroup("Inputs")]
#endif
    [SerializeField]
    private double subDelta = 0.002d;

#if ODIN_INSPECTOR
    [BoxGroup("Inputs")]
#endif
    [SerializeField]
    private Transform follower;

#if ODIN_INSPECTOR
    [BoxGroup("Inputs")]
#endif
    [SerializeField]
    private Transform[] pointsAsTransforms;

    private Vector3[] points;

#if ODIN_INSPECTOR
    [BoxGroup("Follow Mode")]
#endif
    [SerializeField]
    private bool invertSpeed;

#if ODIN_INSPECTOR
    [BoxGroup("Follow Mode")]
#endif
    [SerializeField]

#if ODIN_INSPECTOR
    [PropertyRange(0, nameof(MaxIndex))]
#endif
    private int gotToIndex;

    private int MaxIndex => pointsAsTransforms.Length - 1;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private bool isFollowing;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private double currentSpeedPerSecond;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private double curentIterationPerFrame;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private double pathLength;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private double totalDuration;

#if ODIN_INSPECTOR
    [BoxGroup("State")]
#endif
    [SerializeField]
    private double currentDuration;

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

    private double accumulate = 0;

    private double lastSpeed;

    
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
        state.currentPosition = points[0];
        state.pathIndex = 0;
        state.distanceWalked = 0;


        follower.position = state.currentPosition;
        currentDuration = 0;
        accumulate = 0;

        pathLength = points.LineLength(0, gotToIndex);

        totalDuration = PhysicsUtils.SolveForDurationQuad(speed, pathLength);
        lastSpeed = GetTimeDependentSpeedQuad(totalDuration);
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

        curentIterationPerFrame = 0;

        accumulate += Time.deltaTime;

        // go along using small steps
        while (accumulate >= subDelta && result != WALKING_RESULT.DONE)
        {
            curentIterationPerFrame++;

            currentSpeedPerSecond = GetTimeDependentSpeedQuad(currentDuration);
            
            result = PathUtils.WalkAlongPath(points, currentSpeedPerSecond * subDelta, ref state, gotToIndex);

            accumulate -= subDelta;
            currentDuration += subDelta;
        }

        follower.position = state.currentPosition;
    }

    private double GetTimeDependentSpeedQuad(double time)
    {
        if (invertSpeed)
        {
            double t = (totalDuration - time);

            double invertedSpeed = speed * (t * t);

            return invertedSpeed;
        }

        return speed * (time * time);
    }

    private void OnValidate()
    {
        points = new Vector3[pointsAsTransforms.Length];

        for (int i = 0; i < pointsAsTransforms.Length; i++)
        {
            points[i] = pointsAsTransforms[i].position;
        }
    }

    private void OnDrawGizmos()
    {
        points = new Vector3[pointsAsTransforms.Length];

        for (int i = 0; i < pointsAsTransforms.Length; i++)
        {
            points[i] = pointsAsTransforms[i].position;
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }

    }
}
