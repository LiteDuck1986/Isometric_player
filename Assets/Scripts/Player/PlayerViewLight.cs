using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Light))]
public class PlayerViewLight : MonoBehaviour
{
    public DayNightCycle worldTime;

    [Header("Light")]
    public bool lightAllowed = true;
    public bool onlyWhenDark = true;
    [Min(0f)] public float maximumIntensity = 4f;
    [Min(0.01f)] public float fadeTime = 0.25f;

    private Light viewLight;
    private float fadeVelocity;

    private void Awake()
    {
        viewLight = GetComponent<Light>();
        viewLight.intensity = 0f;
        viewLight.enabled = false;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
            lightAllowed = !lightAllowed;

        float darkness = 1f;

        if (onlyWhenDark && worldTime != null)
        {
            // Starts fading in during twilight.
            darkness = Mathf.InverseLerp(
                0.65f, 0.15f, worldTime.Daylight
            );
        }

        float targetIntensity =
            lightAllowed ? maximumIntensity * darkness : 0f;

        viewLight.intensity = Mathf.SmoothDamp(
            viewLight.intensity,
            targetIntensity,
            ref fadeVelocity,
            fadeTime
        );

        viewLight.enabled =
            targetIntensity > 0.001f || viewLight.intensity > 0.001f;
    }
}