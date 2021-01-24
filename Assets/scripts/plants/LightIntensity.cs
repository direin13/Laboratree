using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class LightIntensity : MonoBehaviour
{
    public int minimumLumen;
    public int maximumLumen;
    private int optimumLumen;
    public int currLumenVal;
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currLumenVal = NumOp.Cutoff(currLumenVal, 0, 30000);
        minimumLumen = NumOp.Cutoff(minimumLumen, 0, minimumLumen);
        maximumLumen = NumOp.Cutoff(maximumLumen, minimumLumen, maximumLumen);
        optimumLumen = minimumLumen + (Math.Abs(maximumLumen - minimumLumen) / 2);

        //Get efficiency of light, the closer to the optimum it is, the higher the efficiency
        float lightEfficiency;
        if (currLumenVal <= optimumLumen)
        {
            lightEfficiency = NumOp.Cutoff((float)(currLumenVal - minimumLumen) / (float)(optimumLumen - minimumLumen), 0, 1f);
        }
        else
        {
            lightEfficiency = NumOp.Cutoff((float)(currLumenVal - maximumLumen) / (float)(optimumLumen - maximumLumen), 0, 1f);
        }

        PlantRates plantRates = GetComponent<PlantRates>();
        plantRates.lightEfficiency = lightEfficiency;
    }
}
