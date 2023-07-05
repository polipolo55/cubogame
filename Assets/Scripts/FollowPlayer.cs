using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 playerPosition;
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.player != null)
        {
            playerPosition = GameManager.Instance.player.transform.position;
            transform.position = playerPosition;
        }
    }
}
