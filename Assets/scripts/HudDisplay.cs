using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class HudDisplay : MonoBehaviour
{
    private Timer gameManagerTimer;

    [SerializeField]
    public Text dateText;
    private int[] monthPeriods;
    // Start is called before the first frame update
    void Start()
    {
        gameManagerTimer = GameObject.Find("GameManager").GetComponent<Timer>();
        monthPeriods = new int[] { 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
    }

    // Update is called once per frame
    void Update()
    {
        int month = 0;
        int currDay = (gameManagerTimer.timeElapsed % 365) + 1;
        bool found = false;

        int dayOfMonth = -1;

        while ( month < monthPeriods.Length && !found )
        {
            int startPeriod;
            int endPeriod = monthPeriods[month];

            if ( month == 0 )
            {
                startPeriod = 0;
            }
            else
            {
                startPeriod = monthPeriods[month - 1];
            }

            if (currDay > startPeriod && currDay <= endPeriod)
            {
                found = true;
                dayOfMonth = (endPeriod - startPeriod) - (endPeriod - currDay);
                //print(startPeriod.ToString() + " " + endPeriod.ToString());
            }
            month++;
        }

        dateText.text = String.Format("Date: {0}/{1}", dayOfMonth.ToString("D2"), month.ToString("D2"));
    }
}
