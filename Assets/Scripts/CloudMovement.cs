using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float playerPos;
    public float distanceDelta;
    private float finalSpeed;
    public float accelDiff;
    public float deccelDiff;
    //private float minDecelMult;
    public float accelMult;


    [Header ("accel")]
    public float accelThreshold;
    public float accelDiv;

    [Space (20)]
    [Header ("deccel")]
    public float decelThreshold;
    public float decelDiv;
    [Space (20)]

    public float speed;

    private Rigidbody2D rb;
    public GameObject SmokePosition;
    
    void Awake()    
    {
        rb = GetComponent<Rigidbody2D>();
        finalSpeed = speed;

    }

    private void Update()
    {
        playerPos = GameManager.Instance.player.transform.position.x;
        distanceDelta = Mathf.Abs(SmokePosition.transform.position.x - playerPos);
        accelDiff = distanceDelta - accelThreshold;
        deccelDiff = decelThreshold - distanceDelta;
        //if(accelDiff > 0) accelMult = 1 + Mathf.Pow(accelDiff / accelDiv, 2);
        //else if (deccelDiff > 0) accelMult = 1 - Mathf.Pow(deccelDiff / decelDiv, 2);

        accelMult = 1;



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.velocity = new Vector2(finalSpeed * accelMult, 0);
    }
}
