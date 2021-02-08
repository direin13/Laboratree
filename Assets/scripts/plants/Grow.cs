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


    public void SetTimeElapsed(int time)
    {
        timeElapsed = NumOp.Cutoff(time, 0, time);
    }


    // Start is called before the first frame update
    void Start()
    {
        Genes genes = GetComponent<Genes>();
        if (genes != null)
        {
            try
            {
                expectedGrowTime = genes.GetValue<int>("expectedGrowTime");
                growthTimeSkew = genes.GetValue<float>("growthTimeSkew");
            }
            catch (Exception e)
            {
                print(e.ToString());
                Debug.LogWarning("A gene could not be read, some variables may be using default values!", gameObject);
            }
        }
        else
        {
            Debug.LogWarning(String.Format("A gene script was not given to '{0}', using default values!", name), gameObject);
        }

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
            int stageInterval = NumOp.Cutoff(currGrowTime, 0, currGrowTime) / growthStages;
            if (stageInterval <= 0)
                growthAmount = 1f;
            else
            {
                int stage = timeElapsed / stageInterval;
                growthAmount = NumOp.Cutoff((float)stage / growthStages, 0f, 1f);
            }
        }

        if (transform.root.gameObject.GetComponent<PlantRates>().PlantAlive())
        {
            //constant countdown
            if (GetComponent<Timer>().Tick())
            {
                timeElapsed = timeElapsed + 1;
            }
        }

        if (debug)
        {
            print(String.Format("Name: {0}, Full Growth Time: {1}hrs, TimeElapsed: {2}hrs", name, currGrowTime, timeElapsed));
        }

    }
}
