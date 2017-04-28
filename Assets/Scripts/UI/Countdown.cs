using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

    public float CountdownFrom;
    public Text textbox;

    private float time = 0;

    private void Start()
    {
        textbox = GetComponent<Text>();
    }

    void Update()
    {       
        time = CountdownFrom - Time.timeSinceLevelLoad;

        if (time <= 0f)
        {
            textbox.text = "";
            TimeUp();
        }else
        {         
            textbox.text = ((int)time).ToString();
        }
    }

    void TimeUp()
    {
        // this function is called when the timer runs out
        //Debug.Log("time is up");
    }
}
