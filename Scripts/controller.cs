using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour
{
    public float player_speed=5f;
    public float game_start_time = 5f;
    public int lives =3;
    public float boundary_left = -Screen.width/2;
    public float boundary_right = Screen.width/2;
    public float boundary_up = Screen.height/2;
    public float boundary_bottom = -Screen.height/2;
    public float enemy_speed=3f;
//    public float number_of_waves=3;
    public float time_for_next_wave=10f;
    public int initial_enemy_count=3;
    public float time_to_survive_each_wave=30f;
    public float bonus_item_lifetime=10f;
    public GameObject Score_text;
    public GameObject wave_time_text;
    public GameObject soul;
    public GameObject Enemy;
    public GameObject life;
    public GameObject shield;
    public GameObject lamp_light;
    public GameObject mines;
    public GameObject[] bonuses = new GameObject[3];
    public AudioSource bgm;
    public GameObject continue_text;


    private float survival_time;
    public float total_survived_time;
    private float time_for_bonus;
    private bool change_difficulty;
    private bool destroy_enemies;
    private bool game_over = false;
    private ArrayList enemies;
    private Stack<GameObject> lives_stack = new Stack<GameObject>();
    private Stack<GameObject> lamps_stack = new Stack<GameObject>();
    private Stack<GameObject> mines_stack = new Stack<GameObject>();

    void Increase_difficulty()
    {
        UnityEngine.Experimental.Rendering.Universal.Light2D light = GameObject.Find("Soul").transform.GetChild(1).gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        light.pointLightOuterRadius = Mathf.Max(light.pointLightOuterRadius-0.5f, 2);
        light.intensity = Mathf.Min(light.intensity - 0.05f, 1f);
        enemy_speed = Mathf.Min(enemy_speed + 0.5f, 7f);
        time_to_survive_each_wave = Mathf.Min(time_to_survive_each_wave + 0.5f, 70f);
        initial_enemy_count = Mathf.Min(initial_enemy_count + 1, 15);
    }

    void Create_Enemies(int count)
    {
        enemies = new ArrayList(count);
        for (int i = 1; i <= count; i++)
        {
            float x = Random.Range(boundary_left, boundary_right);
            float y = Random.Range(boundary_up, boundary_bottom);
            GameObject tmp = (GameObject)Instantiate(Enemy, new Vector2(x, y), Quaternion.identity);
            tmp.GetComponent<enemy_movement>().start_follow = true;
            enemies.Add(tmp);
        }
    }

    void Destroy_enemies()
    {
        if (enemies != null)
        {
            foreach (GameObject tmp in enemies)
            {
                Destroy(tmp);
            }
            enemies = null;
        }
    }

    void add_lives(int count)
    {

        for (int i = 1; i <= count; i++)
        {
            Vector3 life_pos;
            if (lives_stack.Count <= 0)
            {
                life_pos = new Vector3(boundary_right-1f, boundary_bottom+0.5f, -1);
            }
            else
            {
                life_pos = lives_stack.Peek().transform.position + Vector3.left;
            }
            GameObject tmp = Instantiate(life, life_pos, Quaternion.identity);
            lives_stack.Push(tmp);
        }
    }

    void remove_life()
    {
        if (lives_stack.Count > 0) { Destroy(lives_stack.Pop()); }
    }

    public void add_lamps(int count)
    {
        GameObject bl=null;
        foreach (GameObject i in bonuses){
            if(i.name == "bonus_lamp")
            {
                bl = i;
                break;
            }
        }

        for (int i = 1; i <= count; i++)
        {
            Vector3 life_pos;
            if (lamps_stack.Count <= 0)
            {
                life_pos = new Vector3(boundary_left+1f, boundary_bottom+0.5f, -1);
            }
            else
            {
                life_pos = lamps_stack.Peek().transform.position + Vector3.right;
            }
            if (bl != null)
            {
                GameObject tmp = Instantiate(bl, life_pos, Quaternion.identity);
                Destroy(tmp.transform.GetChild(0).gameObject);
                tmp.GetComponent<Collider2D>().enabled = false;
                tmp.GetComponent<bonus_item>().enabled = false;
                lamps_stack.Push(tmp);
            }
            else { Debug.Log("lamp prefab missing"); }
        }
    }

    public bool remove_lamp()
    {
        if (lamps_stack.Count > 0) { Destroy(lamps_stack.Pop()); return true; }
        else { return false; }
    }

    public void add_mines(int count)
    {
        GameObject bl = null;
        foreach (GameObject i in bonuses)
        {
            if (i.name == "bonus_mine")
            {
                bl = i;
                break;
            }
        }

        for (int i = 1; i <= count; i++)
        {
            Vector3 life_pos;
            if (mines_stack.Count <= 0)
            {
                life_pos = new Vector3(boundary_left + 1f, boundary_up + 0.5f, -1);
            }
            else
            {
                life_pos = mines_stack.Peek().transform.position + Vector3.right;
            }
            if (bl != null)
            {
                GameObject tmp = Instantiate(bl, life_pos, Quaternion.identity);
                //Destroy(tmp.transform.GetChild(0).gameObject);
                tmp.GetComponent<Collider2D>().enabled = false;
                tmp.GetComponent<bonus_item>().enabled = false;
                mines_stack.Push(tmp);
            }
            else { Debug.Log("mine prefab missing"); }
        }
    }

    public bool remove_mines()
    {
        if (mines_stack.Count > 0) { Destroy(mines_stack.Pop()); return true; }
        else { return false; }
    }

    // Start is called before the first frame update
    void Start()
    {
        bgm.Play();
        survival_time = time_to_survive_each_wave;
        destroy_enemies = false;
        add_lives(lives);
        time_for_bonus = Random.Range(20f, 30f);
        GameObject player = Instantiate(soul, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        player.name = "Soul";
    }

    // Update is called once per frame
    void Update()
    {
        if (!game_over)
        {
            bool resume = true;
            int check_life = lives;
            bool need_extra_life = (Random.value > 0.5);
            GameObject soul = GameObject.Find("Soul");
            if (soul != null) { lives = soul.GetComponent<player_controller>().lives; resume = soul.GetComponent<player_controller>().resume; }
            else { lives--; }
            if (lives > check_life) { add_lives(lives - check_life); }
            if (lives < check_life) { remove_life(); }
            if (lives > 0)
            {
                if (resume)
                {
                    if (game_start_time <= 0)
                    {
                        wave_time_text.GetComponent<UnityEngine.UI.Text>().text = "";
                        if (enemies == null) { Create_Enemies(initial_enemy_count); }
                        if (survival_time <= 0)
                        {
                            destroy_enemies = true;
                            game_start_time = time_for_next_wave;
                            survival_time = time_to_survive_each_wave;
                        }
                        survival_time -= Time.deltaTime;
                        time_for_bonus -= Time.deltaTime;
                        total_survived_time += Time.deltaTime;
                        Score_text.GetComponent<UnityEngine.UI.Text>().text = "Survival Time: " + total_survived_time.ToString("0.0");
                        if (time_for_bonus <= 0 && need_extra_life == true)
                        {
                            float x = Random.Range(boundary_left, boundary_right);
                            float y = Random.Range(boundary_up, boundary_bottom);
                            Instantiate(bonuses[Random.Range(0, bonuses.Length)], new Vector2(x, y), Quaternion.identity);
                            time_for_bonus = Random.Range(20f, 30f);
                        }
                    }
                    else
                    {
                        game_start_time -= Time.deltaTime;
                        wave_time_text.GetComponent<UnityEngine.UI.Text>().text = game_start_time.ToString("0") + "s to next wave";
                    }
                    if (destroy_enemies) { Increase_difficulty(); Destroy_enemies(); destroy_enemies = false; }
                }
                else { Destroy_enemies(); }
            }
            else
            {
                game_over = true; Debug.Log("Game Over");
            }
        }
        else
        {
            if (continue_text != null)
            {
                float highScore = PlayerPrefs.GetFloat("HighScore");
                string new_high="";
                if (highScore < total_survived_time)
                {
                     new_high = "NEW HighScore!!!";
                     PlayerPrefs.SetFloat("HighScore", total_survived_time);
                }
                Score_text.GetComponent<UnityEngine.UI.Text>().enabled = false;
                continue_text.GetComponent<UnityEngine.UI.Text>().text = "You Died for GOOD!! \n Total Survival Time =  " + total_survived_time.ToString("0.0") + "s\n"+new_high+"\n press R to return to main menu";
                continue_text.GetComponent<UnityEngine.UI.Text>().enabled = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene("Main_Menu");
            }
        }
    }
}
