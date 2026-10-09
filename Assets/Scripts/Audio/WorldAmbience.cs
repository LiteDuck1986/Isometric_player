using UnityEngine;

public class WorldAmbience : MonoBehaviour
{
    public DayNightCycle worldTime;
    public AudioSource daySource;
    public AudioSource nightSource;

    [Range(0f, 1f)] public float dayVolume = 0.25f;
    [Range(0f, 1f)] public float nightVolume = 0.3f;
    [Min(0.01f)] public float fadeSpeed = 0.2f;

    private void Start()
    {
        StartLoop(daySource);
        StartLoop(nightSource);
    }

    private void StartLoop(AudioSource source)
    {
        if (source == null)
            return;

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0f;

        if (source.clip != null)
            source.Play();
    }

    private void Update()
    {
        float daylight = worldTime != null
            ? worldTime.Daylight
            : 1f;

        Fade(daySource, daylight * dayVolume);
        Fade(nightSource, (1f - daylight) * nightVolume);
    }

    private void Fade(AudioSource source, float targetVolume)
    {
        if (source == null)
            return;

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }

    private void OnDisable()
    {
        if (daySource != null)
            daySource.Stop();

        if (nightSource != null)
            nightSource.Stop();
    }

    private void OnEnable()
    {
        // Restart previously initialized loops when re-enabled.
        if (daySource != null && daySource.clip != null)
            daySource.Play();

        if (nightSource != null && nightSource.clip != null)
            nightSource.Play();
    }
}