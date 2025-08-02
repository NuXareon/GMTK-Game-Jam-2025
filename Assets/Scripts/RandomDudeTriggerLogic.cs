using UnityEngine;

public class RandomDudeTriggerLogic : MonoBehaviour
{
    private GameObject Conga;

    private void Awake()
    {
        Conga = GameObject.FindGameObjectWithTag("GameController");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.transform == transform || !enabled)
        {
            return;
        }

        if (other.GetComponent<CongaLeaderMovement>())
        {
            // We collided with the leader!
            Conga.GetComponent<CongaManager>().AddMember(gameObject.transform.parent);
        }
    }
}
