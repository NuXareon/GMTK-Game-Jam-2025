using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMainMenuLevel(14.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadMainMenuLevel(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(0);
    }
}
