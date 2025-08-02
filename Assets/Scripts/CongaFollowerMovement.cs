using UnityEngine;

public class CongaFollowerMovement : MonoBehaviour
{
    // Additional radius used for detecting when conga leader has collected this object
    public float TriggerEndAdditionalRadius = 0.3f;

    [HideInInspector]
    public Transform Following;
    [HideInInspector]
    public float Speed = 5;
    [HideInInspector]
    public float Spacing = 1f;
    [HideInInspector]
    public bool IsJoiningConga = true;
    [HideInInspector]
    public Vector3? Target;
    [HideInInspector]
    public FollowerTriggerLogic TriggerLogic;

    private Animator Anim;
    private CharacterController Controller;
    private float Gravity = 9.81f;
    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Anim = GetComponent<Animator>();

        if (transform.Find("LastMemberCollisionTrigger") == null)
        {
            // Last Member trigger
            GameObject triggerObj = new GameObject("LastMemberCollisionTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;
            triggerObj.transform.localRotation = Quaternion.identity;
            triggerObj.layer = gameObject.layer;

            CharacterController controller = GetComponent<CharacterController>();
            CapsuleCollider capsule = triggerObj.AddComponent<CapsuleCollider>();
            capsule.center = controller.center;
            capsule.radius = controller.radius + TriggerEndAdditionalRadius;
            capsule.height = controller.height;
            capsule.direction = 1;
            capsule.isTrigger = true;

            Rigidbody rb = triggerObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            TriggerLogic = triggerObj.AddComponent<FollowerTriggerLogic>();
            TriggerLogic.enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Don't Start the movement here, we want to give the breadcrum a chance to initialize itself first
    }

    // Update is called once per frame
    void Update()
    {
        if (!Target.HasValue || IsJoiningConga)
        {
            UpdateTargetPosition();
        }
        MoveTowardsTarget();

        UpdateAnimation();

        // Apply gravity (tecnically should be an acceleration, but this is enough to keep our conga in the ground)
        Controller.Move(Vector3.down * Gravity * Time.deltaTime);
    }

    void UpdateTargetPosition()
    {
        if (Following != null)
        {
            BreadcrumTrail Trail = Following.GetComponent<BreadcrumTrail>();
            if (Trail != null)
            {
                if (IsJoiningConga)
                {
                    Target = Trail.GetClosestBreadcrum(transform.position);
                }
                else
                {
                    Target = Trail.ConsumeBreadcrum();
                }
            }
            else
            {
                Debug.LogError("Conga follower ( " + transform.name + " following member " + Following.name + " without a breadcrum trail");
                Target = Following.position;
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (Target.HasValue)
        {
            if (Following == null)
            {
                Debug.LogError("Member (" + transform.name + ") trying to move towards target but following is null");
                Target = null;
            }

            // Keep distance with the following member
            Vector3 FollowingDirection = Following.transform.position - transform.position;
            if (FollowingDirection.sqrMagnitude > Spacing * Spacing)
            {
                // TODO tecnically, instead of clamping movement we should move to the breadcrum and use the remaining movement to go to the next one
                // Calculate distante to target and move
                Vector3 TargetDirection = Target.Value - transform.position;
                TargetDirection.y = 0f;
                Vector3 TargetMove = TargetDirection.normalized * Speed * Time.deltaTime;

                if (TargetMove.sqrMagnitude > TargetDirection.sqrMagnitude)
                {
                    Controller.Move(TargetDirection);
                }
                else
                {
                    Controller.Move(TargetMove);
                }

                // Update rotation
                // TODO Smooth out the rotation
                transform.rotation = Quaternion.LookRotation(TargetDirection.normalized);

                // Check if we reached the target breadcrum
                Vector3 NewTargetDirection = Target.Value - transform.position;
                NewTargetDirection.y = 0f;
                if (NewTargetDirection.sqrMagnitude < 0.1f * 0.1f)
                {
                    Target = null;
                    if (IsJoiningConga)
                    {
                        IsJoiningConga = false;
                        GameObject CongaObj = GameObject.FindGameObjectWithTag("GameController");
                        CongaManager Conga = CongaObj.GetComponent<CongaManager>();
                        Transform Leader = Conga.GetLeader();
                        Physics.IgnoreCollision(Leader.GetComponent<CharacterController>(), Controller, false);
                        BreadcrumTrail Trail = Following.GetComponent<BreadcrumTrail>();
                        if (Trail != null)
                        {
                            Trail.RemoveOldBreadcrums(transform.position);
                        }
                        Following.GetComponent<CongaFollowerMovement>().TriggerLogic.enabled = false;
                        if (Conga.GetLast() == transform)
                        {
                            TriggerLogic.enabled = true;
                        }
                    }
                }
            }
        }
    }

    public void UpdateAnimation()
    {
        if (Following != null)
        {
            if (!GetComponent<CongaLeaderMovement>())
            {
                Anim.SetBool("JoiningConga", IsJoiningConga);
                Anim.SetBool("InConga", !IsJoiningConga);
            }
        }
        else
        {
            Anim.SetBool("JoiningConga", false);
            Anim.SetBool("InConga", false);
        }
    }
}
