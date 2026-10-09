using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;

    [Header("Time")]
    [Range(0f, 24f)] public float hour = 8f;
    [Min(1f)] public float dayLengthMinutes = 10f;
    public bool advanceTime = true;

    [Header("Lighting")]
    public float sunIntensity = 1.2f;
    public Color dayAmbient = new Color(0.5f, 0.55f, 0.6f);
    public Color nightAmbient = new Color(0.025f, 0.035f, 0.06f);
    public Color middaySun = Color.white;
    public Color sunsetSun = new Color(1f, 0.55f, 0.3f);

    public float Daylight { get; private set; }

    private void Start()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;

        if (sun != null)
            RenderSettings.sun = sun;

        ApplyLighting();
    }

    private void Update()
    {
        if (advanceTime)
        {
            hour = Mathf.Repeat(
                hour + Time.deltaTime * 24f
                / (Mathf.Max(1f, dayLengthMinutes) * 60f),
                24f
            );
        }

        ApplyLighting();
    }

    private void ApplyLighting()
    {
        // Sunrise near 06:00; sunset near 18:00.
        float sunAngle = hour / 24f * 360f - 90f;
        float elevation = Mathf.Sin(sunAngle * Mathf.Deg2Rad);

        float twilight = Mathf.InverseLerp(-0.15f, 0.2f, elevation);
        Daylight = Mathf.SmoothStep(0f, 1f, twilight);

        RenderSettings.ambientLight =
            Color.Lerp(nightAmbient, dayAmbient, Daylight);

        if (sun == null)
            return;

        sun.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0f);

        float directLight = Mathf.SmoothStep(
            0f, 1f, Mathf.InverseLerp(0f, 0.5f, elevation)
        );

        sun.intensity = sunIntensity * directLight;
        sun.enabled = sun.intensity > 0.001f;
        sun.color = Color.Lerp(sunsetSun, middaySun, directLight);
    }
}