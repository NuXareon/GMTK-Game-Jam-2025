using UnityEngine;

public class FollowerTriggerLogic : MonoBehaviour
{
    private GameObject Conga;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Conga = GameObject.FindGameObjectWithTag("GameController");
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
            Conga.GetComponent<CongaManager>().OnCongaLoop();
        }
    }
}
