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
    public float currEfficiency;

    public GameObject[] allDependencies;
    public bool debug;


    public DependenceAttribute GetDepComp(GameObject dep)
    {
        return dep.GetComponent<DependenceAttribute>();
    }

    public DependenceAttribute GetDepComp(string name)
    {
        DependenceAttribute dep = null;
        foreach(GameObject obj in allDependencies)
        {
            if (obj.name == name)
                dep = obj.GetComponent<DependenceAttribute>();
        }
        return dep;
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
        //reading gene script for variable values
        Genes genes = GetComponent<Genes>();
        if (genes != null)
        {
            try
            {
                expectedLifetime = Int32.Parse(genes.GetValue("expectedLifetime"));
                deathTimeSkew = float.Parse(genes.GetValue("deathTimeSkew"));
            }
            catch (Exception e)
            {
                print(e);
                Debug.LogWarning("A gene could not be read, some variables may be using default values!", gameObject);
            }
        }
        else
        {
            Debug.LogWarning(String.Format("A gene script was not given to '{0}', using default values!", name), gameObject);
        }


        timeElapsed = 0f;
        timeAliveLeft = expectedLifetime;
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
        deathTimeSkew = NumOp.Cutoff(deathTimeSkew, 0f, 1f);      

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
            currEfficiency = GetCurrentEfficiency(allDependencies);

            //time till death decreases as efficiency lowers whereas it increases growth time
            float actualDeathEffic = (currEfficiency - (currEfficiency * deathTimeSkew));

            timeAliveLeft = (int)((float)expectedLifetime * actualDeathEffic);

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
            print(String.Format("Name: {0}, TimeAlive: {1}hrs, TimeElapsed: {2}hrs", name, timeAliveLeft, timeElapsed));
        }

    }

    public float Health()
    {
        return 1f - ( (float)timeElapsed/(float)timeAliveLeft );
    }
}
