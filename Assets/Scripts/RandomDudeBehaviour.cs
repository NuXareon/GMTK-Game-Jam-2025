using UnityEngine;
using UnityEngine.InputSystem.XR;

public class RandomDudeBehaviour : MonoBehaviour
{
    // Additional radius used for detecting when conga leader has collected this object
    public float TriggerPickupAdditionalRadius = 1.0f;

    private RandomDudeTriggerLogic TriggerLogic;

    private void Awake()
    {
        // Set up triggers
        if (transform.Find("PickUpCollisionTrigger") == null)
        {
            // Pick up trigger
            GameObject triggerObj = new GameObject("PickUpCollisionTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;
            triggerObj.transform.localRotation = Quaternion.identity;
            triggerObj.layer = gameObject.layer;

            CharacterController controller = GetComponent<CharacterController>();
            CapsuleCollider capsule = triggerObj.AddComponent<CapsuleCollider>();
            capsule.center = controller.center;
            capsule.radius = controller.radius + TriggerPickupAdditionalRadius;
            capsule.height = controller.height;
            capsule.direction = 1;
            capsule.isTrigger = true;

            Rigidbody rb = triggerObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            TriggerLogic = triggerObj.AddComponent<RandomDudeTriggerLogic>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        TriggerLogic.enabled = true;
    }

    private void OnDisable()
    {
        TriggerLogic.enabled = false;
    }
}
