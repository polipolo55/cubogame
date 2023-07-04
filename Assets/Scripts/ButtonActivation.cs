using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ButtonActivation : MonoBehaviour
{
    public Animator anim;
    public void Start()
    {
        anim.SetBool("DoorOpen", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") anim.SetBool("DoorOpen", true);
    }
}
