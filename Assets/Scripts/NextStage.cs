using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour
{
    public string levelName;

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player") 
        {
            Debug.Log("TRUCA DOS COPS?");
            if (GameManager.Instance.timer != null)
            {
                float time = GameManager.Instance.timer.TimeOnEnd();
                GameManager.Instance.addTime(time);
            }
            
            nextStage();
        }
    }

    private void nextStage()
    {
        if(GhostRunner.Instance != null) GhostRunner.Instance.stopRecording();
        SceneManager.LoadScene(levelName);
        if (SceneManager.GetActiveScene().name == "End") GameManager.Instance.gamePlayedOnce = true;
    }


}
