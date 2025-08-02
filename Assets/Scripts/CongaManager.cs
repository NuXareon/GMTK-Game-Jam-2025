using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using static Unity.VisualScripting.Metadata;

public class CongaManager : MonoBehaviour
{
    public float AdditionalFollowerSpeed = 1.0f;
    public float FollowerSpacing= 1.0f;
    public List<Transform> Members = new List<Transform>();

    private int FollowerCollisionLayer;
    private int RandomDudeCollisionLayer;

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
        float? LeaderSpeed = null;
        Transform prevChild = null;
        foreach (Transform child in transform)
        {
            CongaFollowerMovement ChildFollowerMovement = child.GetComponent<CongaFollowerMovement>();
            if (ChildFollowerMovement != null)
            {
                if (prevChild == null)
                {
                    Debug.LogError("Failed to find person to follow for member: " + child.name);
                }
                ChildFollowerMovement.Following = prevChild;
                ChildFollowerMovement.Spacing = FollowerSpacing;
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
            else
            {
                CongaLeaderMovement ChildLeaderMovement = child.GetComponent<CongaLeaderMovement>();
                if (ChildLeaderMovement)
                {
                    LeaderSpeed = ChildLeaderMovement.Speed;
                    LeaderController = child.GetComponent<CharacterController>();
                }
            }
            prevChild = child;
        }
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
            Debug.LogWarning("Trying to add new memember (" + NewMember.name + ") to conga which alreadt has Follower Movement Component enabled. Skipping.");
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
        NewMemberRandomDudeBehaviour.enabled = false;

        // Ignore collisions while joining the conga
        Physics.IgnoreCollision(Leader.GetComponent<CharacterController>(), NewMember.GetComponent<CharacterController>());

        Members.Add(NewMember);
    }

    public Transform GetLeader()
    {
        return Members.First();
    }
}
