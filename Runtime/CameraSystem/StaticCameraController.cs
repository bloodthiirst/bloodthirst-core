using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using Bloodthirst.Systems.CameraSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class StaticCameraController : CameraControllerBase<StaticCameraController>, IQuerySingletonPass
{
    [BoxGroup("Horizontal Rotation")]
    [SerializeField]
    private float yRotationValue;

    [BoxGroup("Horizontal Rotation")]
    [SerializeField]
    private float yRotationSensitivity;

    [BoxGroup("Vertical Rotation")]
    [SerializeField]
    private float xRotationSensitivity;

    [BoxGroup("Vertical Rotation")]
    [SerializeField]
    private float xRotationValue;
    [BoxGroup("Vertical Rotation")]
    [SerializeField]
    private Vector2 xRotationMinMax;

    [SerializeField]
    private float speed;

    private float velocityVertical;

    private float velocityHorizontal;

    private MouseUtils _mouseUtils;

    void IQuerySingletonPass.Execute()
    {
        _mouseUtils = BProviderRuntime.Instance.GetSingleton<MouseUtils>();
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        yRotationValue += _mouseUtils.MouseDelta.x * yRotationSensitivity;

        xRotationValue -= _mouseUtils.MouseDelta.y * xRotationSensitivity;

        yRotationValue %= 360;

        xRotationValue = Mathf.Clamp(xRotationValue, xRotationMinMax.x, xRotationMinMax.y);

        velocityVertical = Input.GetAxis("Vertical");

        velocityHorizontal = Input.GetAxis("Horizontal");
    }

    public override void ApplyTransform(out Vector3 position, out Quaternion rotation)
    {
        var rot = Quaternion.Euler(xRotationValue, yRotationValue, 0);

        transform.rotation = rot;

        var vec = transform.forward * velocityVertical;
        vec += transform.right * velocityHorizontal;

        vec.Normalize();

        transform.position += vec * speed * Time.deltaTime;

        position = transform.position;
        rotation = transform.rotation;
    }
}
