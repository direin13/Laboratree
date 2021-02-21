using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class HudDisplay : MonoBehaviour
{
    private Timer gameManagerTimer;

    [SerializeField]
    public Text timeText, dateText;
    // Start is called before the first frame update
    void Start()
    {
        gameManagerTimer = GameObject.Find("GameManager").GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        int time = gameManagerTimer.timeElapsed % 24;
        timeText.text = String.Format( "{0}:00", time.ToString("D2") );

        int days = (gameManagerTimer.timeElapsed / 24) % 30;
        days++;
        int months = (gameManagerTimer.timeElapsed / 730) % 12;
        months++;

        dateText.text = String.Format("{0}/{1}", days.ToString("D2"), months.ToString("D2"));
    }
}
