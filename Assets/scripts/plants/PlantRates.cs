using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MiscFunctions;

public class PlantRates : MonoBehaviour
{
    public int secondsTillDeath;
    public int secondsTillGrowth;
    private int secondsTillGrowthOrigin;
    private int secondsTillDeathOrigin;

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
        secondsTillDeathOrigin = secondsTillDeath;
        secondsTillGrowthOrigin = secondsTillGrowth;

        BalanceFloats(allDependencies, 0, maxEfficiency);

        timeStampObject.GetComponent<Timer>().Set(name + " currentSecondsTillDeath", secondsTillDeath);
        timeStampObject.GetComponent<Timer>().Set(name + " currentSecondsTillGrowth", secondsTillGrowth);
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
        float currEffic = 0f;
        string text = name + " currentSfficiency >> ";
        foreach (GameObject obj in values)
        {
            float subEffic = GetDepComp(obj).dependencyAmount * GetDepComp(obj).dependencyEfficiency;
            currEffic = currEffic + subEffic;
            text = text + String.Format("{0}: {1} ", obj.name, subEffic);
        }
        if (debug)
            print(text + " Current Efficiency: " + currEffic);
        return currEffic;
    }


    // Update is called once per frame
    void Update()
    {

        //get changed dependancy and balance
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
        float currEffic = GetCurrentEfficiency(allDependencies);

        deathTimeSkew = NumOp.Cutoff(deathTimeSkew, 0f, 1f);
        growthTimeSkew = NumOp.Cutoff(growthTimeSkew, 0f, 1f);

        //time till death decreases as efficiency lowers whereas it increases growth time
        float actualDeathEffic = (currEffic - (currEffic * deathTimeSkew));
        float actualGrowthEffic = (currEffic - (currEffic * growthTimeSkew));

        int currentSecondsTillDeath = (int)((float)secondsTillDeath * actualDeathEffic );
        int currentSecondsTillGrowth = secondsTillGrowth + ( secondsTillGrowth - (int)( (float)secondsTillGrowth * actualGrowthEffic) );

        timeStampObject.GetComponent<Timer>().Change(name + " currentSecondsTillDeath", currentSecondsTillDeath);
        timeStampObject.GetComponent<Timer>().Change(name + " currentSecondsTillGrowth", currentSecondsTillGrowth);

        //constant countdown
        if (timeStampObject.GetComponent<Timer>().Tick())
        {
            secondsTillDeath = NumOp.Cutoff(secondsTillDeath-1, 0, secondsTillDeath);
            secondsTillGrowth = NumOp.Cutoff(secondsTillGrowth-1, 0, secondsTillGrowth);
        }

        if (debug)
        {
            timeStampObject.GetComponent<Timer>().PrintTime(name + " currentSecondsTillDeath");
            timeStampObject.GetComponent<Timer>().PrintTime(name + " currentSecondsTillGrowth");
        }
    }
}
