using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MiscFunctions;

public class PlantRates : MonoBehaviour
{
    //unit of time == hours
    public int expectedLifetime;
    private int timeAliveLeft;
    public int expectedGrowTime;
    private int currGrowTime;
    private float timeElapsed;

    //Efficiency of a plant is how close each of it's dependencies efficiency
    //are to 100%. This is calculated by summing up the individual efficiencies
    //and dividing the sum by the max efficiency
    //Dependencies can weigh more than others e.g Aloe's are much more dependent on light
    //than water so plants' dependency may be 90% light vs 10% water

    private readonly float maxEfficiency = 1f;

    //rates used to skew the current time of the death(speed up) and growth(slow down)
    //depending on the level of efficiency of a plant
    public float deathTimeSkew;
    public float growthTimeSkew;
    public GameObject timeStampObject;

    public GameObject[] allDependencies;
    public bool debug;


    public DependenceAttribute GetDepComp(GameObject dep)
    {
        return dep.GetComponent<DependenceAttribute>();
    }

    public Timer TimeStamps()
    {
        return GetComponent<Timer>();
    }

    public float DepRateSum(GameObject [] values)
    {
        float sum = 0;
        foreach (GameObject obj in values)
            sum = sum + GetDepComp(obj).dependencyAmount;

        return sum;
    }


    // Start is called before the first frame update
    void Start()
    {
        timeAliveLeft = expectedLifetime;
        currGrowTime = expectedGrowTime;
        timeElapsed = 0f;

        BalanceFloats(allDependencies, 0, maxEfficiency);
    }


    private void BalanceFloats(GameObject [] values, int pivotIndex, float cutOff)
    {
        //balances array so all the dependency rates add up to the cutOff
        //pivot is the index that the other dependencies get balanced around

        float decValue = DepRateSum(values) - cutOff;

        if (decValue > 0)
        {
            //get max index that's not the pivot, find all indexes with the same
            //value and decrement them

            int maxIndex = -1;
            for (int i = 0; i < values.Length; i++)
            {
                if (i != pivotIndex)
                {
                    if (maxIndex == -1 || GetDepComp(values[i]).dependencyAmount > GetDepComp(values[maxIndex]).dependencyAmount)
                        maxIndex = i;
                }
            }

            if (maxIndex != -1)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (i != pivotIndex)
                    {
                        if (GetDepComp(values[i]).dependencyAmount == GetDepComp(values[maxIndex]).dependencyAmount)
                            GetDepComp(values[i]).dependencyAmount = GetDepComp(values[i]).dependencyAmount - decValue;
                        else if (decValue < 1)
                            GetDepComp(values[i]).dependencyAmount = GetDepComp(values[i]).dependencyAmount - (decValue/2);
                    }
                }
             
                for (int i = 0; i < values.Length; i++)
                    GetDepComp(values[i]).dependencyAmount = NumOp.Cutoff(GetDepComp(values[i]).dependencyAmount, 0f, cutOff);
            }
        }
    }
    

    public float GetCurrentEfficiency(GameObject[] values)
    {
        //Sum up all the efficiencies of the dependencies and return how well plant is doing from 0-1f(0-100%)
        float currEffic = 0f;
        string text = name + " Current Efficiency >> ";
        foreach (GameObject obj in values)
        {
            float subEffic = GetDepComp(obj).dependencyAmount * GetDepComp(obj).dependencyEfficiency;
            currEffic = currEffic + subEffic;
            text = text + String.Format("{0}: {1} ", obj.name, subEffic);
        }
        if (debug)
            print(text + ", Current Efficiency: " + currEffic);
        return currEffic;
    }


    // Update is called once per frame
    void Update()
    {
        //make sure variables stay in acceptable range
        expectedLifetime = NumOp.Cutoff(expectedLifetime, 1, expectedLifetime);
        expectedGrowTime = NumOp.Cutoff(expectedGrowTime, 1, expectedGrowTime);
        deathTimeSkew = NumOp.Cutoff(deathTimeSkew, 0f, 1f);
        growthTimeSkew = NumOp.Cutoff(growthTimeSkew, 0f, 1f); 
        

        //get changed dependancy and balance based on the change
        int changedIndex = -1;

        for (int i=0; i < allDependencies.Length; i++)
        {
            if (GetDepComp(allDependencies[i]).DepAmountChanged())
                changedIndex = i;
        }

        if (changedIndex != -1)
        {
            BalanceFloats(allDependencies, changedIndex, maxEfficiency);
        }

        if (timeElapsed < timeAliveLeft)
        {
            float currEffic = GetCurrentEfficiency(allDependencies);

            //time till death decreases as efficiency lowers whereas it increases growth time
            float actualDeathEffic = (currEffic - (currEffic * deathTimeSkew));
            float actualGrowthEffic = (currEffic - (currEffic * growthTimeSkew));

            timeAliveLeft = (int)((float)expectedLifetime * actualDeathEffic);
            currGrowTime = expectedGrowTime + (expectedGrowTime - (int)((float)expectedGrowTime * actualGrowthEffic));

            //constant countdown
            if (TimeStamps().Tick())
            {
                timeElapsed = timeElapsed + 1;
            }
        }
        else
        {
            TimeToColor[] allColors = GetComponentsInChildren<TimeToColor>();
            for (int i=0; i < allColors.Length; i++)
            {
                allColors[i].alphaValue = 0.25f;
            }
        }

        if (debug)
        {
            print("Plant Time Left (%): " + Health().ToString());
            print(String.Format("Name: {3}, TimeAlive: {0}hrs, Full Growth Time: {1}hrs, TimeElapsed: {2}hrs", timeAliveLeft, currGrowTime, timeElapsed, name));
        }

    }

    public float GrowthAmount(float numOfStages, int timeElapsed)
    {
        float growthAmount;
        if (numOfStages <= 0)
        {
            growthAmount = 1f;
        }
        else
        {
            float stageInterval = NumOp.Cutoff((float)currGrowTime, 0f, (float)currGrowTime) / numOfStages;
            float stage = (float)timeElapsed / stageInterval;
            growthAmount = NumOp.Cutoff(stage / numOfStages, 0f, 1f);
        }

        return growthAmount;
    }

    public float Health()
    {
        return 1f - ( (float)timeElapsed/(float)timeAliveLeft );
    }
}
