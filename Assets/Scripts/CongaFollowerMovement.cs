using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class CongaFollowerMovement : MonoBehaviour
{
    //[HideInInspector]
    public Transform Following;
    [HideInInspector]
    public float Speed = 5;
    [HideInInspector]
    public float Spacing = 1f;

    private Vector3? Target;
    private CharacterController Controller;
    private float Gravity = 9.81f;
    public bool IsJoiningConga = false;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Don't Start the movement here, we want to give the breadcrum a chance to initialize itself first
        IsJoiningConga = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Target.HasValue || IsJoiningConga)
        {
            UpdateTargetPosition();
        }
        MoveTowardsTarget();

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
                        GameObject Conga = GameObject.FindGameObjectWithTag("GameController");
                        Transform Leader = Conga.GetComponent<CongaManager>().GetLeader();
                        Physics.IgnoreCollision(Leader.GetComponent<CharacterController>(), Controller, false);
                        BreadcrumTrail Trail = Following.GetComponent<BreadcrumTrail>();
                        if (Trail != null)
                        {
                            Trail.RemoveOldBreadcrums(transform.position);
                        }
                    }
                }
            }
        }
    }
}
