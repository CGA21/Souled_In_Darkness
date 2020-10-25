using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    public Animator anim;
    public Transform player;
    public int lives;
    public GameObject continue_text;
    public bool resume = true;
    public AudioSource death;

    private float speed;
    private bool run;
    private float boundary_horizontal;
    private float boundary_vertical;
    // Start is called before the first frame update
    void Start()
    {
        run = false;
        speed = GameObject.Find("GameManager").GetComponent<controller>().player_speed;
        lives = GameObject.Find("GameManager").GetComponent<controller>().lives;
        boundary_horizontal = GameObject.Find("GameManager").GetComponent<controller>().boundary_right;
        boundary_vertical = GameObject.Find("GameManager").GetComponent<controller>().boundary_up;
        continue_text = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (resume)
        {
            continue_text.GetComponent<UnityEngine.UI.Text>().enabled = false;
            if (Input.GetKey(KeyCode.D))
            {
                player.GetChild(0).rotation = new Quaternion(0, 0, 0, 0);
                player.position += player.right * speed * Time.deltaTime;
                run = true;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                player.GetChild(0).rotation = new Quaternion(0, 180, 0, 0);
                player.position -= player.right * speed * Time.deltaTime;
                run = true;
            }
            if (Input.GetKey(KeyCode.W))
            {
                player.position += player.up * speed * Time.deltaTime;
                run = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                player.position -= player.up * speed * Time.deltaTime;
                run = true;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GameObject.Find("GameManager").GetComponent<controller>().remove_lamp())
                {
                    GameObject lamp = GameObject.Find("GameManager").GetComponent<controller>().lamp_light;
                    Instantiate(lamp, player.position, Quaternion.identity);
                }
                else { Debug.Log("No Lamps!!"); }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (GameObject.Find("GameManager").GetComponent<controller>().remove_mines())
                {
                    GameObject mines = GameObject.Find("GameManager").GetComponent<controller>().mines;
                    Instantiate(mines, player.position, Quaternion.identity);
                }
                else { Debug.Log("No Mines!!"); }
            }
        }
        else
        {
            if (Input.anyKey)
            {
                resume = true;
                player.position = new Vector2(0, 0);
                transform.GetChild(1).gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = true;
            }
        }
        if(player.position.x < -boundary_horizontal) { player.position += Vector3.right * 0.1f; }
        else if(player.position.x > boundary_horizontal) { player.position -= Vector3.right * 0.1f; }
        if (player.position.y < -boundary_vertical) { player.position += Vector3.up * 0.1f; }
        else if (player.position.y > boundary_vertical) { player.position -= Vector3.up * 0.1f; }
        anim.SetBool("start_run", run);
        run = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            if (lives > 1)
            {
                death.Play();
                lives--;
                resume = false;
                anim.SetBool("death", true);
                anim.SetBool("death", false);
                if (continue_text != null)
                {
                    continue_text.GetComponent<UnityEngine.UI.Text>().text = lives.ToString() + " lives left \n (press any key to continue)";
                    continue_text.GetComponent<UnityEngine.UI.Text>().enabled = true;
                }
                else { Debug.Log("no text to show"); }
                transform.GetChild(1).gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = false;
            }
            else
            {
                death.Play();
                lives--;
                Destroy(this.gameObject,0.5f);
            }
        }
        if (collision.gameObject.tag == "boundary")
        {
            Debug.Log("Boundary Hit");
        }
        if (collision.gameObject.tag == "extra_life")
        {
            lives++;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "shield")
        {
            Destroy(collision.gameObject);
            Vector3 v_tmp = player.transform.position + Vector3.forward;
            GameObject tmp = GameObject.Find("GameManager").GetComponent<controller>().shield;
            GameObject sh = Instantiate(tmp, v_tmp, Quaternion.identity) as GameObject;
            sh.transform.parent = player;
        }
        if(collision.gameObject.tag == "lamp")
        {
            Destroy(collision.gameObject);
            GameObject.Find("GameManager").GetComponent<controller>().add_lamps(1);
        }
        if (collision.gameObject.tag == "mine_bonus")
        {
            Destroy(collision.gameObject);
            GameObject.Find("GameManager").GetComponent<controller>().add_mines(1);
        }
    }
}
