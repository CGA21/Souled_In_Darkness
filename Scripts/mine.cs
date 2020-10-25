using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mine : MonoBehaviour
{
    public Animator anim;
    public AudioSource blast;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
          //  this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
          //  this.gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = false;
            anim.SetBool("blast", true);
            this.gameObject.GetComponent<Collider2D>().enabled = false;
            blast.Play();
            Destroy(this.gameObject, 1f);
        }
    }
}
