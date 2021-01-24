using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MiscFunctions;

public class PlantRates : MonoBehaviour
{
    public int SecondsTillDeath; //seconds
    private int currentSecondsTillDeath;

    private readonly float maxEfficiency = 1f;
    public float lightDependanceRate;
    public float soilDependanceRate;
    public float tempDependanceRate;
    public float wateringDependanceRate;
    private float[] allRates;


    //effiencies represent how much each dependancy is being used
    public float lightEfficiency;
    public float soilEfficiency;
    public float tempEfficiency;
    public float wateringEfficiency;

    private float [] BalanceFloats(float [] values, float pivotIndex, float cutOff)
    {
        //balances array so all the values add up to the cutOff
        //pivot is the index that the other values get balanced around

        float decValue = values.Sum() - cutOff;

        if (decValue > 0)
        {
            //get max index that's not the pivot, find all indexes with the same
            //value and decrement them

            int maxIndex = -1;
            for (int i = 0; i < values.Length; i++)
            {
                if (i != pivotIndex)
                {
                    if (maxIndex == -1 || values[i] > values[maxIndex])
                        maxIndex = i;
                }
            }

            if (maxIndex != -1)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (i != pivotIndex)
                    {
                        if (values[i] == values[maxIndex])
                            values[i] = values[i] - decValue;
                        else if (decValue < 1)
                            values[i] = values[i] - (decValue/2);
                    }
                }
             
                for (int i = 0; i < values.Length; i++)
                    values[i] = NumOp.Cutoff(values[i], 0f, cutOff);
            }
        }

        return values;
    }

    // Start is called before the first frame update
    void Start()
    {
        allRates = new[] { lightDependanceRate, soilDependanceRate, tempDependanceRate, wateringDependanceRate };
        BalanceFloats(allRates, allRates.Max(), maxEfficiency);
    }

    

    public float GetDepRate(int index)
    {
        if (index == 0)
            return lightDependanceRate;
        else if (index == 1)
            return soilDependanceRate;
        else if (index == 2)
            return tempDependanceRate;
        else if (index == 3)
            return wateringDependanceRate;
        else
            return -1f;
    }

    public float GetCurrentEfficiency()
    {
        float lightRate = GetDepRate(0) * lightEfficiency;
        float soilRate = GetDepRate(1) * soilEfficiency;
        float tempRate = GetDepRate(2) * tempEfficiency;
        float waterRate = GetDepRate(3) * wateringEfficiency;

        float currEffic = lightRate + soilRate + tempRate + waterRate;
        if (name == "Aloe")
            print(String.Format("LightRate: {0}, SoilRate: {1}, tempRate: {2}, waterRate: {3}, Total Efficiency: {4}", 
                                                                                                                    lightRate, 
                                                                                                                    soilRate, 
                                                                                                                    tempRate, 
                                                                                                                    waterRate, 
                                                                                                                    currEffic));
        return currEffic;
    }

    // Update is called once per frame
    void Update()
    {
        lightEfficiency = NumOp.Cutoff(lightEfficiency, 0f, 1f);
        soilEfficiency = NumOp.Cutoff(soilEfficiency, 0f, 1f);
        tempEfficiency = NumOp.Cutoff(tempEfficiency, 0f, 1f);
        wateringEfficiency = NumOp.Cutoff(wateringEfficiency, 0f, 1f);

        //get changed dependancy and balance
        float changedIndex = -1;
        for (int i=0; i<allRates.Length; i++)
        {
            if (allRates[i] != GetDepRate(i))
                changedIndex = i;
            allRates[i] = GetDepRate(i);
        }

        if (changedIndex != -1f)
        {
            BalanceFloats(allRates, changedIndex, maxEfficiency);
            lightDependanceRate = allRates[0];
            soilDependanceRate = allRates[1];
            tempDependanceRate = allRates[2];
            wateringDependanceRate = allRates[3];
        }
        float currEffic = GetCurrentEfficiency();

        currentSecondsTillDeath = (int)((float)SecondsTillDeath * currEffic);
        if (name == "Aloe")
            print("Second left till death: " + currentSecondsTillDeath.ToString());
    }
}
