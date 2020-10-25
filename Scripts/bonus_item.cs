using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bonus_item : MonoBehaviour
{
    private float die_time;
    private bool text_present;
    private GameObject txt = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        die_time = GameObject.Find("GameManager").GetComponent<controller>().bonus_item_lifetime;
        if (this.transform.childCount >= 2)
        {
            Transform tmp = this.transform.GetChild(1);
            if (tmp != null) { txt = (GameObject)tmp.GetChild(0).gameObject; }
        }
        if (txt!=null) { text_present = true; }
        else { text_present = false; }
    }
    // Update is called once per frame
    void Update()
    {
        if (die_time <= 0) { Destroy(this.gameObject); }
        else { die_time -= Time.deltaTime; }
        if (text_present)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position) + Vector3.down * 25;
            txt.GetComponent<UnityEngine.UI.Text>().rectTransform.position = pos;
            txt.GetComponent<UnityEngine.UI.Text>().text = die_time.ToString("0");
        }
    }
}
