using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]

    private PlayerStats playerStatsComponent;

    private PlayerStats.stats playerStats;

    private Transform cameraTransform;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;
    public float jumpHeight = 1.0f;

    [Header("Animation Settings")]
    public float animationSmooth = 5f;

    [Header("Animation Script")]
    public TriggerActions triggerActions;

    private CharacterController controller;
    private Animator anim;
    private Vector3 inputDirection;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private float verticalVelocity = 0f;
    private bool externalOverride = false;
    private Vector3 externalVelocity;
    private bool isMovementLocked = false;

    public Vector3 directionOverride = Vector3.zero;
    private PlayerLife playerLife;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerLife = GetComponent<PlayerLife>();
        playerStatsComponent = GetComponent<PlayerStats>();
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        playerStats = playerStatsComponent.GetModifiedStats();
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");


        inputDirection = new Vector3(moveX, 0.0f, moveZ).normalized;

        if (playerLife.IsDead()) {
            return;
        }
        Move();
        RotateCharacter();
        UpdateAnimator();
        externalOverride = false;
        directionOverride = Vector3.zero;
    }

    void Move()
    {
        moveDirection = cameraTransform.TransformDirection(inputDirection);
        moveDirection.y = 0.0f;
        moveDirection.Normalize();

        if (!controller.isGrounded)
            verticalVelocity -= gravity * Time.deltaTime;
        if (isMovementLocked && !externalOverride)
        {
            currentVelocity = Vector3.zero;
            controller.Move(Vector3.down * gravity * Time.deltaTime);
            return;
        }

        if (externalOverride)
        {
            Vector3 motionVector = externalVelocity;
            motionVector.y = verticalVelocity;
            controller.Move(motionVector * Time.deltaTime);

            return;
        }

        Vector3 targetVelocity = moveDirection * playerStats.speed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 20f);
        Vector3 motion = currentVelocity;
        motion.y = verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }

    public void resetPositionAt(Vector3 position)
    {
        controller.enabled = false;
        verticalVelocity = 0.0f;
        currentVelocity = Vector3.zero;
        transform.position = position;
        controller.transform.position = position;
        controller.enabled = true;
    }

    public void SetMovementLock(bool locked)
    {
        isMovementLocked = locked;
    }

    public void OverrideMovement(Vector3 velocity, float yVelocity, bool applyGravity)
    {
        externalOverride = true;
        externalVelocity = velocity;

        if (applyGravity) {
            verticalVelocity += yVelocity;
        }
    }

    public void OverrideRotation(Vector3 direction)
    {
        directionOverride = direction;
    }

    void UpdateAnimator()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);
        float speed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;

        anim.SetFloat("Velocity X", localVelocity.x * 2f);
        anim.SetFloat("Velocity Z", localVelocity.z * 2f);
        anim.SetBool("Moving", speed > 0.1f);
    }

    void RotateCharacter()
    {
        if (currentVelocity.magnitude > 0.1f || directionOverride != Vector3.zero)
        {
            Vector3 direction = directionOverride != Vector3.zero ? directionOverride : currentVelocity;
            direction.y = 0.0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "Ground")
            return;
        if (triggerActions != null && triggerActions.GetCurrentTriggerNumber() == 3)
        {
            if (triggerActions.GetAnimationCompletion() < 30f) {
                triggerActions.CancelCurrentTriggerAndActivate(8);
            }
        } else if (triggerActions == null)
        {
            Debug.LogWarning("TriggerActions script is not provided on the Entity player, some animation events might not work.");
        }
    }
}
