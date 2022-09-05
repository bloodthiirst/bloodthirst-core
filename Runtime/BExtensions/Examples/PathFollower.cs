using Bloodthirst.Scripts.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static Bloodthirst.Scripts.Utils.PathUtils;

public class PathFollower : MonoBehaviour
{

    [BoxGroup("Inputs")]
    [SerializeField]
    private double speed;

    [BoxGroup("Inputs")]
    [SerializeField]
    private double subDelta = 0.002d;

    [BoxGroup("Inputs")]
    [SerializeField]
    private Transform follower;

    [BoxGroup("Inputs")]
    [SerializeField]
    private Transform[] pointsAsTransforms;

    private Vector3[] points;

    [BoxGroup("Follow Mode")]
    [SerializeField]
    private bool invertSpeed;

    [BoxGroup("Follow Mode")]
    [SerializeField]
    [PropertyRange(0, nameof(MaxIndex))]
    private int gotToIndex;

    private int MaxIndex => pointsAsTransforms.Length - 1;

    [BoxGroup("State")]
    [SerializeField]
    private bool isFollowing;

    [BoxGroup("State")]
    [SerializeField]
    private double currentSpeedPerSecond;

    [BoxGroup("State")]
    [SerializeField]
    private double curentIterationPerFrame;

    [BoxGroup("State")]
    [SerializeField]
    private double pathLength;

    [BoxGroup("State")]
    [SerializeField]
    private double totalDuration;

    [BoxGroup("State")]
    [SerializeField]
    private double currentDuration;

    [BoxGroup("State")]
    [SerializeField]
    private WALKING_RESULT result;

    [BoxGroup("State")]
    [SerializeField]
    private WalkingState state;

    private double accumulate = 0;

    private double lastSpeed;

    [Button]
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

    [Button]
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
