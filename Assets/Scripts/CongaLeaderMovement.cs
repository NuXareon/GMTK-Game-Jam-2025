using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CongaLeaderMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float TurnSpeed = 360f;

    private float Gravity = 9.81f;

    private InputSystem_Actions InputActions;
    private CharacterController Controller;
    private Vector2 PlayerInputDirection;
    private Vector3 MovingDirection;
    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputActions = new InputSystem_Actions();
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
        Controller.Move(transform.forward * Speed * Time.deltaTime);

        // Apply gravity (tecnically should be an acceleration, but this is enough to keep our conga in the ground)
        Controller.Move(Vector3.down * Gravity * Time.deltaTime);
    }
}
