using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_manager: MonoBehaviour
{
    public GameObject Main, help_text, help_objects,Score;
    public AudioSource bgm;

    private void Start()
    {
        bgm.Play();
        Score.GetComponent<UnityEngine.UI.Text>().text = "HighScore : " + PlayerPrefs.GetFloat("HighScore").ToString("0.00")+"s";
        Debug.Log(PlayerPrefs.GetFloat("HighScore").ToString());
    }
    public void play_button()
    {
        SceneManager.LoadScene("Scene_01");
        Debug.Log("play_pressed");
    }

    public void help_button()
    {
        Main.SetActive(false);
        help_objects.SetActive(true);
        help_text.SetActive(true);
        
    }
    public void back_button()
    {
        Main.SetActive(true);
        help_objects.SetActive(false);
        help_text.SetActive(false);
    }
    public void quit_button()
    {
        Debug.Log("quit_pressed");
        Application.Quit();
    }
}
