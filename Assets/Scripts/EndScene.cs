using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    public float eventTimer = 0;
    public string levelName;
    // Start is called before the first frame update

    void Start()
    {
        float sum = GameManager.Instance.totalTime();
        Debug.Log(sum);
    }

    // Update is called once per frame
    void Update()
    {
        eventTimer += Time.deltaTime;

        GameManager.Instance.player.canInput = false;

        if (eventTimer > 0.5f && eventTimer < 0.6f) 
        {
            GameManager.Instance.player.OnJumpInput();
        }
        if(eventTimer > 0.5f) GameManager.Instance.player.moveInput.x = 1;

        if(eventTimer > 15f) SceneManager.LoadScene(levelName);


    }
}
