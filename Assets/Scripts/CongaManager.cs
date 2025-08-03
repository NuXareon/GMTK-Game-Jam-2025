using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class CongaManager : MonoBehaviour
{
    public float AdditionalFollowerSpeed = 1.0f;
    public float FollowerSpacing = 1.0f;
    public int FollowersReleasedOnCollision = 1;
    public List<Transform> Members = new List<Transform>();
    public float ExtremeModeModifier = 2f;

    private int FollowerCollisionLayer;
    private int RandomDudeCollisionLayer;
    private float? LeaderSpeed = null;

    void Awake()
    {
        FollowerCollisionLayer = LayerMask.NameToLayer("CongaFollower");
        RandomDudeCollisionLayer = LayerMask.NameToLayer("RandomDude");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.childCount < 3)
        {
            Debug.LogError("Minimum size of conga has to be at least 3, current:  " + transform.childCount);
        }

        CharacterController LeaderController = null;
        Transform prevChild = null;
        foreach (Transform child in transform)
        {
            CongaLeaderMovement ChildLeaderMovement = child.GetComponent<CongaLeaderMovement>();
            CongaFollowerMovement ChildFollowerMovement = child.GetComponent<CongaFollowerMovement>();
            if (ChildLeaderMovement)
            {
                LeaderSpeed = ChildLeaderMovement.Speed;
                LeaderController = child.GetComponent<CharacterController>();
            }
            else if (ChildFollowerMovement)
            {
                if (prevChild == null)
                {
                    Debug.LogError("Failed to find person to follow for member: " + child.name);
                }
                ChildFollowerMovement.Following = prevChild;
                ChildFollowerMovement.Spacing = FollowerSpacing;
                ChildFollowerMovement.IsJoiningConga = false;
                ChildFollowerMovement.TriggerLogic.enabled = false;
                if (LeaderSpeed.HasValue)
                {
                    ChildFollowerMovement.Speed = LeaderSpeed.Value + AdditionalFollowerSpeed;
                }
                else
                {
                    Debug.LogError("Failed to assign speed to member: " + child.name);
                }
                if (LeaderController != null)
                {
                    // Disable collision with leader for the initial conga. Should only be really needed for the second conga member.
                    CharacterController myController = child.GetComponent<CharacterController>();
                    Physics.IgnoreCollision(LeaderController, myController);
                }
            }
            prevChild = child;
        }

        Members.First().GetComponent<CongaFollowerMovement>().TriggerLogic.enabled = false;
        Members.Last().GetComponent<CongaFollowerMovement>().TriggerLogic.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddMember(Transform NewMember)
    {
        CongaFollowerMovement NewMemberFollowerMovement = NewMember.GetComponent<CongaFollowerMovement>();
        if (NewMemberFollowerMovement == null)
        {
            Debug.LogError("Trying to add new memember (" + NewMember.name + ") to conga without a Follower Movement Component.");
            return;
        }

        if (NewMemberFollowerMovement.enabled)
        {
            Debug.LogWarning("Trying to add new memember (" + NewMember.name + ") to conga which already has Follower Movement Component enabled. Skipping.");
            return;
        }

        Transform Leader = Members.First();
        CongaLeaderMovement LeaderMovement = Leader.GetComponent<CongaLeaderMovement>();
        Transform CurrentLast = Members.Last();

        NewMember.SetParent(transform);
        NewMember.gameObject.layer = FollowerCollisionLayer;

        NewMemberFollowerMovement.Following = CurrentLast;
        NewMemberFollowerMovement.Spacing = FollowerSpacing;
        NewMemberFollowerMovement.Speed = LeaderMovement.Speed + AdditionalFollowerSpeed;

        NewMemberFollowerMovement.enabled = true;

        RandomDudeBehaviour NewMemberRandomDudeBehaviour = NewMember.GetComponent<RandomDudeBehaviour>();
        if (NewMemberRandomDudeBehaviour)
        {
            NewMemberRandomDudeBehaviour.enabled = false;
        }

        CongaLeaderMovement NewMemberLeaderMovement = NewMember.GetComponent<CongaLeaderMovement>();
        if (NewMemberLeaderMovement)
        {
            NewMemberLeaderMovement.enabled = false;
        }

        // Ignore collisions while joining the conga
        Physics.IgnoreCollision(Leader.GetComponent<CharacterController>(), NewMember.GetComponent<CharacterController>());

        Members.Add(NewMember);
    }

    public void OnCongaCollision(RaycastHit hit)
    {
        for (int i = 0; i < FollowersReleasedOnCollision; ++i)
        {
            if (Members.Count == 3)
            {
                break;
            }
            ReleaseLastMember();
        }
    }

    public Transform GetLeader()
    {
        return Members.First();
    }

    public Transform GetLast()
    {
        return Members.Last();
    }

    private void ReleaseLastMember()
    {
        Transform CurrentLast = Members.Last();

        RandomDudeBehaviour CurrentLastRandomDudeBehaviour = CurrentLast.GetComponent<RandomDudeBehaviour>();
        if (CurrentLastRandomDudeBehaviour == null)
        {
            Debug.LogError("Trying to remove memember (" + CurrentLast.name + ") to conga without a Random Dude Behaviour.");
            return;
        }

        CurrentLast.SetParent(null);
        CurrentLast.gameObject.layer = RandomDudeCollisionLayer;

        CongaFollowerMovement CurrentLastMemberFollowerMovement = CurrentLast.GetComponent<CongaFollowerMovement>();
        CurrentLastMemberFollowerMovement.TriggerLogic.enabled = false;
        CurrentLastMemberFollowerMovement.Following.GetComponent<CongaFollowerMovement>().TriggerLogic.enabled = true;
        CurrentLastMemberFollowerMovement.Following = null;
        CurrentLastMemberFollowerMovement.Target = null;
        CurrentLastMemberFollowerMovement.IsJoiningConga = true;
        CurrentLastMemberFollowerMovement.UpdateAnimation();
        CurrentLastMemberFollowerMovement.enabled = false;

        CurrentLastRandomDudeBehaviour.enabled = true;

        Members.Remove(CurrentLast);
    }

    public void OnCongaLoop()
    {
        Transform Leader = GetLeader();
        CongaLeaderMovement LeaderMovement = Leader.GetComponent<CongaLeaderMovement>();
        if (LeaderMovement.PostCollisionTime > 0f)
        {
            // Mo winning while invulnerable
            return;
        }

        Debug.Log("Conga Loop!");
        AddMember(Leader);
        CongaFollowerMovement LeaderFollowerMovement = Leader.GetComponent<CongaFollowerMovement>();
        // Do a bit of conga
        LeaderFollowerMovement.Speed = LeaderMovement.Speed/2f;
        LeaderFollowerMovement.Spacing = 0.1f;
        LeaderFollowerMovement.IsJoiningConga = false;

        FindFirstObjectByType<LevelFlow>().FinishLevel();
    }

    public void EnterExtremeMode()
    {
        CongaLeaderMovement LeaderMovement = GetLeader().GetComponent<CongaLeaderMovement>();
        LeaderMovement.Speed = LeaderSpeed.Value * ExtremeModeModifier;
        float ExtremeLeaderSpeed = LeaderMovement.Speed;
        GetLeader().GetComponent<Animator>().speed = 3f;

        CongaFollowerMovement[] AllFollowers = FindObjectsByType<CongaFollowerMovement>(FindObjectsSortMode.None);
        foreach (CongaFollowerMovement ChildFollowerMovement in AllFollowers)
        {
            if (ChildFollowerMovement.gameObject.GetComponent<CongaLeaderMovement>())
            {
                continue;
            }
            ChildFollowerMovement.Speed = ExtremeLeaderSpeed + AdditionalFollowerSpeed;
            ChildFollowerMovement.Anim.speed = 3f;
            ChildFollowerMovement.Anim.SetBool("Dance", true);
        }
    }

    public void ExitExtremeMode()
    {
        CongaLeaderMovement LeaderMovement = GetLeader().GetComponent<CongaLeaderMovement>();
        LeaderMovement.Speed = LeaderSpeed.Value;
        GetLeader().GetComponent<Animator>().speed = 1f;

        CongaFollowerMovement[] AllFollowers = FindObjectsByType<CongaFollowerMovement>(FindObjectsSortMode.None);
        foreach (CongaFollowerMovement ChildFollowerMovement in AllFollowers)
        {
            if (ChildFollowerMovement.gameObject.GetComponent<CongaLeaderMovement>())
            {
                continue;
            }
            ChildFollowerMovement.Speed = LeaderSpeed.Value + AdditionalFollowerSpeed;
            ChildFollowerMovement.Anim.speed = 1f;
            ChildFollowerMovement.Anim.SetBool("Dance", false);
        }
    }
}
