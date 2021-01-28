using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class PlantManager : MonoBehaviour
{
    public float globalTimeSpeed;
    public GameObject[] plantCollection;
    public GameObject[] inLabSpace;
 
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        globalTimeSpeed = NumOp.Cutoff(globalTimeSpeed, 0.00001f, 1f);
        foreach (GameObject plant in plantCollection)
        {
            if (plant != null)
            {
                plant.GetComponent<Timer>().speed = 0;
            }
        }

        foreach (GameObject plant in inLabSpace)
        {
            if (plant != null)
            {
                plant.GetComponent<Timer>().speed = globalTimeSpeed/0.000001f;
            }
        }
    }
}
