using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.Utils;
using Bloodthirst.Systems.CameraSystem;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEngine;

public class StaticCameraController : CameraControllerBase<StaticCameraController>
{
#if ODIN_INSPECTOR
    [BoxGroup("Horizontal Rotation")]
#endif
    [SerializeField]
    private float yRotationValue;

#if ODIN_INSPECTOR
    [BoxGroup("Horizontal Rotation")]
#endif
    [SerializeField]
    private float yRotationSensitivity;

#if ODIN_INSPECTOR
    [BoxGroup("Vertical Rotation")]
#endif
    [SerializeField]
    private float xRotationSensitivity;

#if ODIN_INSPECTOR
    [BoxGroup("Vertical Rotation")]
#endif
    [SerializeField]
    private float xRotationValue;

#if ODIN_INSPECTOR
    [BoxGroup("Vertical Rotation")]
#endif
    [SerializeField]
    private Vector2 xRotationMinMax;

    [SerializeField]
    private float speed;

    private float velocityVertical;

    private float velocityHorizontal;

    private MouseUtils _mouseUtils;


    public override void OnRegister(CameraManager cameraManager)
    {
        base.OnRegister(cameraManager);
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

    public override void OnCameraControllerSelected(bool isImmidiate)
    {

    }
}
