using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (!GameManager.Instance.TutorialPlayed)
        {
            SceneManager.LoadScene("Tutorial");
            GameManager.Instance.TutorialPlayed = true;
        }
        else SceneManager.LoadScene("Level1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
