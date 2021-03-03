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
    public bool readGenesOnStart;


    public void ReadGenesOnStart(bool b)
    {
        readGenesOnStart = b;
    }

    public bool PlantAlive()
    {
        return (GetComponent<Timer>().timeElapsed < timeAliveLeft);
    }
    public DependenceAttribute GetDepComp(GameObject dep)
    {
        if (dep)
            return dep.GetComponent<DependenceAttribute>();
        else
            return null;
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

    // Start is called before the first frame update
    void Start()
    {

        //reading gene script for variable values
        if (readGenesOnStart)
        {
            Genes genes = GetComponent<Genes>();
            if (genes != null)
            {
                try
                {
                    expectedLifetime = genes.GetValue<int>("expectedLifetime");
                    deathTimeSkew = genes.GetValue<float>("deathTimeSkew");
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
            BalanceFloats(allDependencies, 0, maxEfficiency);
        }

        timeAliveLeft = expectedLifetime;
    }

    private void BalanceFloats(GameObject [] values, int pivotIndex, float cutOff)
    {
        //balances array so all the dependency rates add up to the cutOff
        //pivot is the index that the other dependencies get balanced around

        float decValue = 0;

        foreach (GameObject obj in values)
            decValue = decValue + GetDepComp(obj).dependencyAmount;


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

        float actualDeathEffic = 0f;

        if (PlantAlive())
        {
            currEfficiency = NumOp.Cutoff(GetCurrentEfficiency(allDependencies), 0f, 1f);

            //time till death decreases as efficiency lowers whereas it increases growth time

            //the higher the death skew, the more exponentially the currefficiency falls
            actualDeathEffic = currEfficiency - ( currEfficiency * (deathTimeSkew * (1f - currEfficiency)) );
            timeAliveLeft = (int)((float)expectedLifetime * actualDeathEffic);

            TimeToColor[] allColors = GetComponentsInChildren<TimeToColor>();
            for (int i = 0; i < allColors.Length; i++)
            {
                allColors[i].alphaValue = 1f;
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
            print("actual death efficiency: " + actualDeathEffic.ToString());
            print("Plant Time Left (%): " + Health().ToString());
            print("Plant alive: " + PlantAlive().ToString());
            print(String.Format("Name: {0}, TimeAlive: {1}hrs, TimeElapsed: {2}hrs", name, timeAliveLeft, GetComponent<Timer>().timeElapsed));
        }

    }

    public float Health()
    {
        int timeElapsed = GetComponent<Timer>().timeElapsed;
        return 1f - ( (float)timeElapsed/(float)timeAliveLeft );
    }
}
