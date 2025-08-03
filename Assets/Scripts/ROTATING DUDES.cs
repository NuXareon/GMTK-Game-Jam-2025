using UnityEngine;

public class ROTATINGDUDES : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f); // degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
