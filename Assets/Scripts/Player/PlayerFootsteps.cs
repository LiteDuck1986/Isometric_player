using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip[] walkClips;
    public AudioClip[] runClips;

    [Header("Distance between foot contacts")]
    [Min(0.1f)] public float walkStepDistance = 0.9f;
    [Min(0.1f)] public float runStepDistance = 1.3f;
    public float runSpeedThreshold = 3f;

    [Header("Sound")]
    [Range(0f, 1f)] public float walkVolume = 0.45f;
    [Range(0f, 1f)] public float runVolume = 0.7f;

    private CharacterController controller;
    private AudioSource source;
    private float distanceSinceStep;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = 1f;
    }

    private void LateUpdate()
    {
        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;

        if (!controller.isGrounded || speed < 0.1f)
        {
            distanceSinceStep = 0f;
            return;
        }

        bool running = speed > runSpeedThreshold;
        float stepDistance = running
            ? runStepDistance
            : walkStepDistance;

        distanceSinceStep += speed * Time.deltaTime;

        if (distanceSinceStep < stepDistance)
            return;

        distanceSinceStep %= stepDistance;

        AudioClip[] clips = running && runClips != null
            && runClips.Length > 0 ? runClips : walkClips;

        if (clips == null || clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null)
            return;

        source.pitch = Random.Range(0.95f, 1.05f);
        source.PlayOneShot(
            clip, running ? runVolume : walkVolume
        );
    }
}