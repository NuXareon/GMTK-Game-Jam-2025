using UnityEngine;
using System.Collections;
using TMPro;

public class MusicManager : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public AudioSource MusicSource;
    public AudioSource ExtremeMusicSource;
    public float ExtremeMusicPeriod = 20f;
    public float ExtremeMusicDuration = 10f;
    private CongaManager Conga;

    private void Awake()
    {
        Conga = GameObject.FindGameObjectWithTag("GameController").GetComponent<CongaManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ExtremeMusicPeriod > 5f)
        {
            StartCoroutine(QueueNextExtremeMusic(ExtremeMusicPeriod));
        }
        else if (ExtremeMusicPeriod > 0f)
        {
            Debug.LogWarning("ExtremeMusicPeriod should be bigger than 5, or 0 if we don't want it");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator QueueNextExtremeMusic(float seconds)
    {
        yield return new WaitForSeconds(seconds-5f);
        Text.text = "Get Ready...";
        Text.gameObject.SetActive(true);
        StartCoroutine(QueueExtremeMusicStart(5f));
    }

    IEnumerator QueueExtremeMusicStart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Text.text = "DANCE!";
        MusicSource.Stop();
        ExtremeMusicSource.Play();
        Conga.EnterExtremeMode();
        StartCoroutine(QueueExtremeMusicStop(ExtremeMusicDuration));
    }

    IEnumerator QueueExtremeMusicStop(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Text.gameObject.SetActive(false);
        ExtremeMusicSource.Stop();
        MusicSource.Play();
        Conga.ExitExtremeMode();
        StartCoroutine(QueueNextExtremeMusic(ExtremeMusicPeriod));
    }

    public void OnFinishLevel()
    {
        StopAllCoroutines();
        Text.gameObject.SetActive(false);
    }
}
