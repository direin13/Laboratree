using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiscFunctions;
using System;


public class Grow : MonoBehaviour
{
    public int expectedGrowTime;
    private int currGrowTime;
    public float growthTimeSkew;
    public int growthStages;
    public bool debug;
    public int timeElapsed;
    public float growthAmount;

    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0;
        currGrowTime = expectedGrowTime;
    }

    // Update is called once per frame
    void Update()
    {
        //make sure variables stay in acceptable range
        expectedGrowTime = NumOp.Cutoff(expectedGrowTime, 1, expectedGrowTime);
        growthTimeSkew = NumOp.Cutoff(growthTimeSkew, 0f, 1f);
        growthStages = NumOp.Cutoff(growthStages, 0, growthStages);

        PlantRates pr = transform.root.gameObject.GetComponent<PlantRates>();

        //time till growth increases as efficiency lowers
        float actualGrowthEffic = (pr.currEfficiency - (pr.currEfficiency * growthTimeSkew));

        currGrowTime = expectedGrowTime + (expectedGrowTime - (int)((float)expectedGrowTime * actualGrowthEffic));

        if (growthStages <= 0)
        {
            growthAmount = 1f;
        }
        else
        {
            float stageInterval = NumOp.Cutoff((float)currGrowTime, 0f, (float)currGrowTime) / growthStages;
            int stage = timeElapsed / (int)stageInterval;
            growthAmount = NumOp.Cutoff((float)stage / growthStages, 0f, 1f);
        }


        //constant countdown
        if (GetComponent<Timer>().Tick())
        {
            timeElapsed = timeElapsed + 1;
        }

        if (debug)
        {
            print(String.Format("Name: {0}, Full Growth Time: {1}hrs, TimeElapsed: {2}hrs", name, currGrowTime, timeElapsed));
        }

    }
}
