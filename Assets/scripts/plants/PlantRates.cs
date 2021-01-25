using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MiscFunctions;

public class PlantRates : MonoBehaviour
{
    public int SecondsTillDeath; //seconds
    public GameObject[] allDependencies;
    private int currentSecondsTillDeath;
    private readonly float maxEfficiency = 1f;


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
        BalanceFloats(allDependencies, 0, maxEfficiency);
    }


    private void BalanceFloats(GameObject [] values, int pivotIndex, float cutOff)
    {
        //balances array so all the values add up to the cutOff
        //pivot is the index that the other values get balanced around

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
        string text = "";
        foreach (GameObject obj in values)
        {
            currEffic = currEffic + (GetDepComp(obj).dependencyAmount * GetDepComp(obj).dependencyEfficiency);
            text = text + String.Format("{0}: {1} ", obj.name, GetDepComp(obj).dependencyAmount * GetDepComp(obj).dependencyEfficiency);
        }
        if (name == "Aloe")
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

        currentSecondsTillDeath = (int)((float)SecondsTillDeath * currEffic);
        if (name == "Aloe")
            print("Second left till death: " + currentSecondsTillDeath.ToString());
    }
}
