using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    public float eventTimer = 0;
    // Start is called before the first frame update

    void Start()
    {

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


    }
}
