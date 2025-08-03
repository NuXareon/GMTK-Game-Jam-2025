using Unity.Burst;
using UnityEngine;

public class CongaLeaderMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float TurnSpeed = 360f;
    public float HeadOnCollisionAngle = 30f;
    public float HeadOnCollisionAngleFollower = 45f;
    public float CollisionInvulnerabilityTime = 2f;

    [HideInInspector]
    public float PostCollisionTime = 0f;

    private float Gravity = 9.81f;
    private float CollisionTime = 0f;

    private InputSystem_Actions InputActions;
    private CharacterController Controller;
    private CongaManager Conga;
    private Vector2 PlayerInputDirection;
    private Vector3 MovingDirection;
    private int LeaderLayer;
    private int FollowerLayer;
    private Material[] Mat;
    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputActions = new InputSystem_Actions();
        Conga = GameObject.FindGameObjectWithTag("GameController").GetComponent<CongaManager>();
        Mat = GetComponentInChildren<SkinnedMeshRenderer>().materials;

        LeaderLayer = LayerMask.NameToLayer("CongaLeader");
        FollowerLayer = LayerMask.NameToLayer("CongaFollower");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputActions.Player.Move.Enable();
        InputActions.Player.Move.performed += ctx => PlayerInputDirection = ctx.ReadValue<Vector2>();
        InputActions.Player.Move.canceled += ctx => PlayerInputDirection = Vector2.zero;
        MovingDirection = transform.forward;
    }

    void Update()
    {
        if (CollisionTime > 0f)
        {
            // During collision stop movement
            CollisionTime -= Time.deltaTime;
            CollisionTime = Mathf.Max(CollisionTime, 0f);
            return;
        }

        if (PostCollisionTime > 0f)
        {
            Conga.SetCongaColor(Color.darkRed);

            PostCollisionTime -= Time.deltaTime;
            PostCollisionTime = Mathf.Max(PostCollisionTime, 0f);

            if (PostCollisionTime == 0)
            {
                // Restore collisions
                Physics.IgnoreLayerCollision(LeaderLayer, FollowerLayer, false);
                Conga.SetCongaColor(Color.white);
            }
        }

        // Process Player Input
        Vector3 PlayerTargetMovement = new Vector3(PlayerInputDirection.x, 0, PlayerInputDirection.y);
        if (PlayerTargetMovement != Vector3.zero)
        {
            PlayerTargetMovement.Normalize();
            float maxRadiansDelta = TurnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            MovingDirection = Vector3.RotateTowards(MovingDirection, PlayerTargetMovement, maxRadiansDelta, 0f);
        }

        // Update Rotation
        transform.rotation = Quaternion.LookRotation(MovingDirection);

        // Move Forward
        CollisionFlags flags = Controller.Move(transform.forward * Speed * Time.deltaTime);

        // Detect if the movement hit something
        if ((flags & CollisionFlags.Sides) != 0 && PostCollisionTime == 0f)
        {
            Vector3 origin = transform.position + Vector3.up * (Controller.height / 2f);
            Vector3 direction = transform.forward;
            float RaycastLength = 1.0f;
            LayerMask collisionLayers = 1 | 1 << FollowerLayer; // Default + Follower
            Debug.DrawLine(origin, origin + direction * RaycastLength);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, RaycastLength, collisionLayers))
            {
                float CollisionAngle = HeadOnCollisionAngle;
                if (hit.collider.gameObject.GetComponent<CongaFollowerMovement>() != null)
                {
                    CollisionAngle = HeadOnCollisionAngleFollower;
                }

                // Collision - check angle for heads-on collision
                float angle = Vector3.Angle(transform.forward, -hit.normal);
                if (angle < CollisionAngle)
                {
                    // Play fall animation
                    GetComponent<Animator>().SetTrigger("Fall");

                    // Disable collision between leader and followers
                    Physics.IgnoreLayerCollision(LeaderLayer, FollowerLayer, true);

                    CollisionTime = 1f;
                    PostCollisionTime = CollisionInvulnerabilityTime;

                    // Last follower is released
                    Conga.OnCongaCollision(hit);
                }   
            }
        }

        // Apply gravity (tecnically should be an acceleration, but this is enough to keep our conga in the ground)
        Controller.Move(Vector3.down * Gravity * Time.deltaTime);
    }
}
