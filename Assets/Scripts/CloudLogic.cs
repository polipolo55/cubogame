using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudLogic : MonoBehaviour
{
    // Start is called before the first frame update

    private float playerPos;
    private float distance;
    private float accelDiff;
    private float deccelDiff;
    private float accelMult;

    public float speed;



    [Header("accel")]
    public float accelThreshold;
    public float accelDiv;



    [Space(20)]
    [Header("deccel")]
    public float deccelThreshold;
    public float deccelDiv;

    private Rigidbody2D rb;
    public GameObject gas;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GameManager.Instance.player.transform.position.x;
        distance = playerPos - gas.transform.position.x;

        accelDiff = (playerPos - accelThreshold) - gas.transform.position.x;
        deccelDiff = gas.transform.position.x - (playerPos - deccelThreshold);

        if (accelDiff > 0) accelMult = 1 + Mathf.Pow(accelDiff / accelDiv, 2);
        else if (deccelDiff > 0) accelMult = 1 - Mathf.Pow(deccelDiff / deccelDiv, 2);
        else accelMult = 1;



    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(speed * accelMult, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3((playerPos - accelThreshold), -10, 0), new Vector3((playerPos - accelThreshold), 10, 0));
        Gizmos.DrawLine(new Vector3((playerPos - deccelThreshold), -10, 0), new Vector3((playerPos - deccelThreshold), 10, 0));
    }

}
