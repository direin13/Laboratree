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
    //could be somewhere between the min and max. This would be the optimum value

    public float dependencyAmount;
    private float prevDepAmount;
    private bool depAmountChanged;

    public float dependencyEfficiency;
    public float minValue;
    public float maxValue;
    public float optimumValue;
    public float currValue;
    // Start is called before the first frame update

    void Start()
    {
        depAmountChanged = false;
        prevDepAmount = dependencyAmount;
    }

    // Update is called once per frame
    void Update()
    {
        currValue = NumOp.Cutoff(currValue, 0, 100000000);
        minValue = NumOp.Cutoff(minValue, 0, minValue);
        maxValue = NumOp.Cutoff(maxValue, minValue, maxValue);
        if (optimumValue < minValue || optimumValue > maxValue)
            optimumValue = minValue + (Math.Abs(maxValue - minValue) / 2);
        //Get efficiency of light, the closer to the optimum it is, the higher the efficiency
        if (currValue <= optimumValue)
        {
            dependencyEfficiency = NumOp.Cutoff((float)(currValue - minValue) / (float)(optimumValue - minValue), 0, 1f);
        }
        else
        {
            dependencyEfficiency = NumOp.Cutoff((float)(currValue - maxValue) / (float)(optimumValue - maxValue), 0, 1f);
        }
        if (dependencyAmount != prevDepAmount)
        {
            depAmountChanged = true;
            prevDepAmount = dependencyAmount;
        }
    }

    public bool DepAmountChanged()
    {
        bool out_ = depAmountChanged;
        depAmountChanged = false;
        return out_;
    }
}
