using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player") 
        {
            float time = GameManager.Instance.timer.TimeOnEnd();
            GameManager.Instance.addTime(time);
            nextStage();
        }
    }

    private void nextStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


}
