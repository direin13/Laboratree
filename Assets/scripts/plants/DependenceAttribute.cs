using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class DependenceAttribute : MonoBehaviour
{
    //Dependecy is an attribute that affects the plants overall efficiency
    //Each dependency has an optimum value which represents the best case scenario
    //for that dependency, for example if a dependency was light intensity, an optimum range
    //could be somewhere between the min and max. By default it's half way.

    public float dependencyAmount;
    private float prevDepAmount;
    private bool depAmountChanged;

    public float dependencyEfficiency;
    public float minValue;
    public float maxValue;
    public float optimumPercentage;
    public float currValue;

    public bool readGenesOnStart;


    public void ReadGenesOnStart(bool b)
    {
        readGenesOnStart = b;
    }


    public bool debug;
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
                    dependencyAmount = genes.GetValue<float>("dependencyAmount");
                    minValue = genes.GetValue<int>("minValue");
                    maxValue = genes.GetValue<int>("maxValue");
                    optimumPercentage = genes.GetValue<float>("optimumPercentage");
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
        }

        depAmountChanged = false;
        prevDepAmount = dependencyAmount;
    }


    // Update is called once per frame
    void Update()
    {
        currValue = NumOp.Cutoff(currValue, 0, 100000000);
        minValue = NumOp.Cutoff(minValue, 0, minValue);
        maxValue = NumOp.Cutoff(maxValue, minValue, maxValue);
        optimumPercentage = NumOp.Cutoff(optimumPercentage, 0f, 1f);
        float optimumValue = minValue + ((maxValue - minValue) * optimumPercentage);
        optimumValue = NumOp.Cutoff(optimumValue, minValue, maxValue);

        currValue = optimumValue;

        //Get efficiency of dependency, the closer to the optimum it is, the higher the efficiency
        if (currValue <= optimumValue)
        {
            dependencyEfficiency = NumOp.Cutoff((float)(currValue - minValue) / (float)(optimumValue - minValue), 0, 1f);
        }
        else
        {
            dependencyEfficiency = NumOp.Cutoff((float)(maxValue - currValue) / (float)(maxValue - optimumValue), 0, 1f);
        }
        if (dependencyAmount != prevDepAmount)
        {
            depAmountChanged = true;
            prevDepAmount = dependencyAmount;
        }

        if (debug)
        {
            print("Optimum Value: " + optimumValue.ToString());
            print("Dependency efficieiency: " + dependencyEfficiency.ToString());
        }
    }

    public bool DepAmountChanged()
    {
        bool out_ = depAmountChanged;
        depAmountChanged = false;
        return out_;
    }
}
