using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Keyboard
        if (Keyboard.current != null)
        {
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key != null && key.wasPressedThisFrame)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    return;
                }
            }
        }

        // Handle gamepad?
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonNorth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame  ||
                Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                return;
            }
        }
    }
}
