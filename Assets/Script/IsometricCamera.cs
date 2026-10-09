using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class IsometricCamera : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float pitch = 35.264f;
    public float yaw = 45f;
    public float distance = 15f;
    public float focusHeight = 1f;

    [Header("Zoom")]
    [Min(0.1f)] public float minZoom = 3f;
    [Min(0.1f)] public float maxZoom = 12f;
    [Tooltip("Size change per scroll unit. Adjust for your mouse or trackpad.")]
    [Min(0f)] public float zoomSensitivity = 0.01f;
    [Min(0.01f)] public float zoomSmoothTime = 0.15f;

    private Camera viewCamera;
    private float targetZoom;
    private float zoomVelocity;

    private void OnEnable()
    {
        viewCamera = GetComponent<Camera>();
        viewCamera.orthographic = true;
        targetZoom = Mathf.Clamp(viewCamera.orthographicSize, minZoom, maxZoom);
        viewCamera.orthographicSize = targetZoom;
        zoomVelocity = 0f;
    }

    private void OnValidate()
    {
        minZoom = Mathf.Max(0.1f, minZoom);
        maxZoom = Mathf.Max(minZoom, maxZoom);
        zoomSmoothTime = Mathf.Max(0.01f, zoomSmoothTime);
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
            return;

        // Scroll is already a frame delta; do not multiply it by deltaTime.
        float scroll = mouse.scroll.ReadValue().y;
        targetZoom = Mathf.Clamp(
            targetZoom - scroll * zoomSensitivity, minZoom, maxZoom
        );
    }

    private void LateUpdate()
    {
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        viewCamera.orthographicSize = Mathf.Clamp(
            Mathf.SmoothDamp(
                viewCamera.orthographicSize, targetZoom, ref zoomVelocity,
                zoomSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime
            ),
            minZoom, maxZoom
        );

        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = target.position + Vector3.up * focusHeight;
        transform.SetPositionAndRotation(
            focus - rotation * Vector3.forward * distance, rotation
        );
    }
}
