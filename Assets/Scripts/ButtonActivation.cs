using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ButtonActivation : MonoBehaviour
{
    public Animator anim;
    public Animator but;
    public void Start()
    {
        anim.SetBool("DoorOpen", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            anim.SetBool("DoorOpen", true);
            but.SetBool("green", true);
            Debug.Log("a");

        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            but.SetBool("green", false);
        }
    }
}
