using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class IsometricPlayer : MonoBehaviour
{
    public Camera viewCamera;
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float turnSpeed = 720f;
    public float gravity = -20f;

    private CharacterController controller;
    private float verticalSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (viewCamera == null)
            viewCamera = Camera.main;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (viewCamera == null)
            return;

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        Vector2 input = Vector2.zero;
        bool wantsRun = false;

        if (keyboard != null)
        {
            input.x = (keyboard.dKey.isPressed ? 1f : 0f)
                    - (keyboard.aKey.isPressed ? 1f : 0f);

            input.y = (keyboard.wKey.isPressed ? 1f : 0f)
                    - (keyboard.sKey.isPressed ? 1f : 0f);

            wantsRun = keyboard.leftShiftKey.isPressed
                    || keyboard.rightShiftKey.isPressed;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        // Movement remains relative to the camera.
        Vector3 forward = viewCamera.transform.forward;
        Vector3 right = viewCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.y + right * input.x;
        bool moving = moveDirection.sqrMagnitude > 0.001f;
        bool aiming = mouse != null && mouse.rightButton.isPressed;

        Vector3 desiredFacing = transform.forward;

        if (aiming)
        {
            // Project the cursor onto a horizontal plane at foot level.
            Ray ray = viewCamera.ScreenPointToRay(
                mouse.position.ReadValue()
            );

            Plane aimPlane = new Plane(Vector3.up, transform.position);

            if (aimPlane.Raycast(ray, out float distance))
            {
                Vector3 toCursor = ray.GetPoint(distance)
                                 - transform.position;
                toCursor.y = 0f;

                // Avoids unstable rotation when the cursor is at feet level.
                if (toCursor.sqrMagnitude > 0.01f)
                    desiredFacing = toCursor.normalized;
            }
        }
        else if (moving)
        {
            desiredFacing = moveDirection.normalized;
        }

        // While right-click is held, forward sprinting takes priority.
        // Positive dot product means movement toward the cursor-facing direction.
        bool forwardSprint = aiming
            && wantsRun
            && moving
            && Vector3.Dot(moveDirection.normalized, desiredFacing) > 0.1f;

        if (forwardSprint)
        {
            aiming = false;
            desiredFacing = moveDirection.normalized;
        }

        // Rotates the unanimated Player root
        if (aiming || moving)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(desiredFacing, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        bool movingBackward = false;

        if (aiming && moving)
        {
            Vector3 movement = moveDirection.normalized;

            // Check both current facing and intended cursor-facing.
            // This also prevents backward sprinting during a turn.
            movingBackward =
                Vector3.Dot(movement, transform.forward) < -0.001f
                || Vector3.Dot(movement, desiredFacing) < -0.001f;
        }

        bool running = wantsRun && moving && !movingBackward;
        float speed = running ? runSpeed : walkSpeed;

        if (controller.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalSpeed;

        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            int aimLayer = animator.GetLayerIndex("UpperBodyAim");

            if (aimLayer > 0)
            {
                float targetWeight = aiming ? 1f : 0f;

                float weight = Mathf.MoveTowards(
                    animator.GetLayerWeight(aimLayer),
                    targetWeight,
                    8f * Time.deltaTime
                );

                animator.SetLayerWeight(aimLayer, weight);
            }

            Vector3 actualVelocity = controller.velocity;
            actualVelocity.y = 0f;

            // Immediately removes the run blend when backing up.
            if (movingBackward)
            {
                animator.SetFloat(
                    "Speed",
                    Mathf.Min(actualVelocity.magnitude, walkSpeed)
                );
            }
            else
            {
                animator.SetFloat(
                    "Speed",
                    actualVelocity.magnitude,
                    0.1f,
                    Time.deltaTime
                );
            }
        }
    }
}