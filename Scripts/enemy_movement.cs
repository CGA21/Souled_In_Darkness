using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_movement : MonoBehaviour
{
    public Animator anim;
    public bool start_follow=false;
    public AudioSource attack;

    private float enemy_speed;
    private Vector3 distance;
    private float displacement;
    private GameObject player;
    private float follow_distance;
    private void Start()
    {
        enemy_speed = GameObject.Find("GameManager").GetComponent<controller>().enemy_speed;
        player = GameObject.Find("Soul");
        follow_distance = 10f;
    }
    // Update is called once per frame
    void Update()
    {
        if (player != null && start_follow)
        {
            displacement = Vector3.Distance(player.transform.position, this.transform.position);
            if (displacement <= follow_distance)
            {
                distance = player.transform.position - this.transform.position;
                if (player.transform.position.x < this.transform.position.x)
                {
                    this.transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    this.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                if (displacement <= 2 && player !=null)
                {
                    anim.SetBool("attack", true);
                    attack.Play();
                }
                else
                {
                    anim.SetBool("attack", false);
                }
                this.GetComponent<Rigidbody2D>().AddForce(distance * enemy_speed * Time.deltaTime);
            }
        }
        else
        {
            anim.SetBool("attack", false);
        }
    }
}
