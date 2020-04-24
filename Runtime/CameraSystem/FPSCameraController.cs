using Assets.Scripts.Core.Utils;
using Bloodthirst.Systems.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : CameraControllerBase<FPSCameraController>
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float distanceFromTarget;

    [SerializeField]
    private float yOffsetValue;

    [SerializeField]
    private float yRotationValue;

    [SerializeField]
    private float yRotationSensitivity;

    [SerializeField]
    private float xRotationValue;

    [SerializeField]
    private Transform focusPoint;

    public Transform FocusPoint { get => focusPoint; set => focusPoint = value; }

    private Vector3 gizmozPos;

    public void Bind(Transform target)
    {
        Debug.Log("FPS Camera Binding reached");

        this.target = target;
    }


    private void Update()
    {
        if (!isEnabled)
            return;

        yRotationValue += MouseUtils.Instance.MouseDelta.x * yRotationSensitivity;

        yRotationValue %= 360;


    }

    public override void ApplyTransform(out Vector3 position, out Quaternion rotation)
    {
        target.rotation = Quaternion.Euler(0, yRotationValue, 0);

        Vector3 pos = target.position;

        pos += -target.forward * distanceFromTarget;

        pos += Vector3.up * yOffsetValue;

        Vector3 viewDir = (target.position - pos).normalized;

        Vector3 upwards = Vector3.Cross(-target.right, viewDir).normalized;

        Quaternion rot = Quaternion.LookRotation(viewDir, upwards);

        pos = AvoidObstacle(pos, focusPoint.position);

        gizmozPos = pos;

        position = pos;
        rotation = rot;
    }

    private void OnDrawGizmos()
    {
        if (focusPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(focusPoint.position, gizmozPos);
    }

    Vector3 AvoidObstacle(Vector3 camera, Vector3 target)
    {
        Vector3 start = camera;

        RaycastHit hit;

        if (Physics.Linecast(target, camera, out hit))
        {
            start = hit.point;
        }

        return start;

    }
}
