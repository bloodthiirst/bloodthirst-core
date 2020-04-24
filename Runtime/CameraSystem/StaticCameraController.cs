using Bloodthirst.Systems.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraController : CameraControllerBase<StaticCameraController>
{
    public override void ApplyTransform(out Vector3 position, out Quaternion rotation)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}
