using UnityEngine;
using TMPro;
public class PulseText : MonoBehaviour
{
    public float pulseSpeed = 2f;       // Speed of the pulse
    public float pulseScale = 0.1f;     // How much to scale (e.g. 0.1 = ±10%)
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        transform.localScale = originalScale * scaleFactor;
    }
}
