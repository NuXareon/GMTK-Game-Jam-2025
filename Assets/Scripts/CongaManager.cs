using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static Unity.VisualScripting.Metadata;

public class CongaManager : MonoBehaviour
{
    private int Size = 3;

    public float AdditionalFollowerSpeed = 1.0f;
    public List<Transform> Members = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Size = transform.childCount;

        if (Size < 3)
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
}
