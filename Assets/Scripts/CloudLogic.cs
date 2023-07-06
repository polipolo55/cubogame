using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudLogic : MonoBehaviour
{
    // Start is called before the first frame update

    private float playerPos;
    private float distance;
    private float accelDiff;
    private float deccelDiff;
    private float accelMult;

    public float speed;
    public float deathTime;
    private float deathTimer;
    private bool countDown = false;



    [Header("accel")]
    public float accelThreshold;
    public float accelDiv;



    [Space(20)]
    [Header("deccel")]
    public float deccelThreshold;
    public float deccelDiv;

    private Rigidbody2D rb;
    public GameObject gas; 
    public GameObject deathMask;
    public Material vignette;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Color color = vignette.color;
        color.a = 0.2f;
        vignette.color = color;
        deathTimer = deathTime;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(countDown)
        {
            deathTimer -= Time.deltaTime;
        }
        
        playerPos = GameManager.Instance.player.transform.position.x;
        distance = playerPos - gas.transform.position.x;

        accelDiff = (playerPos - accelThreshold) - gas.transform.position.x;
        deccelDiff = gas.transform.position.x - (playerPos - deccelThreshold);

        if (accelDiff > 0) accelMult = 1 + Mathf.Pow(accelDiff / accelDiv, 2);
        else if (deccelDiff > 0) 
        {
            accelMult = 1 - Mathf.Pow(deccelDiff / deccelDiv, 2);
            Color color = vignette.color;
            color.a = 0.2f + (deccelDiff / deccelThreshold) * 0.8f;
            vignette.color = color;
            GameManager.Instance.cameraShake.ShakeWithDuration(0.1f);
        } 
        else accelMult = 1;

        if(deathTimer < 0)
        {
            GameManager.Instance.player.canInput = false;
            GameManager.Instance.player.isAlive = false;
            GameManager.Instance.player.rb.velocity = Vector2.zero;
            if(deathMask.transform.localScale.x > 0) deathMask.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }


        }
        else if(deathMask.transform.localScale.x < 95)
        {
            deathMask.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }




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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") countDown = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        countDown = false;
        if (collision.tag == "Player") deathTimer = deathTime;
    }




}
