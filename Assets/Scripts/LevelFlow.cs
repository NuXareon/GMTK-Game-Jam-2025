using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelFlow : MonoBehaviour
{
    public TextMeshProUGUI InitialCountdown;
    public TextMeshProUGUI CongaLoop;
    public int CountdownStart = 3;

    private int CurrentCountdown;
    private float InitialFontSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Freeze all movement until level starts
        InitialCountdown.gameObject.SetActive(false);
        CongaLoop.gameObject.SetActive(false);
        Time.timeScale = 0f;
        CurrentCountdown = CountdownStart;
        InitialFontSize = InitialCountdown.fontSize;
        StartCoroutine(HandleCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (InitialCountdown.IsActive())
        {
            InitialCountdown.fontSize -= 100f*Time.unscaledDeltaTime;
            InitialCountdown.fontSize = Mathf.Max(InitialCountdown.fontSize, InitialFontSize-150f);
        }
    }

    IEnumerator HandleCountdown()
    {
        InitialCountdown.text = CurrentCountdown.ToString();
        InitialCountdown.fontSize = InitialFontSize;
        InitialCountdown.gameObject.SetActive(true);
        while (CurrentCountdown > 0)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            CurrentCountdown--;
            InitialCountdown.fontSize = InitialFontSize;
            InitialCountdown.text = CurrentCountdown.ToString();
        }
        InitialCountdown.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void FinishLevel()
    {
        CongaLoop.gameObject.SetActive(true);
        StartCoroutine(LoadNextLevel(3f));
        GetComponent<MusicManager>().OnFinishLevel();
    }

    IEnumerator LoadNextLevel(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
