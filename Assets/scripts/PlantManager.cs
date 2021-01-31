using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class PlantManager : MonoBehaviour
{
    //This is the script that manages all the plants in the game
    public float globalTimeSpeed;
    public static List<GameObject> plantCollection;
    public readonly Dictionary<string, bool> plantStatus = new Dictionary<string, bool>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject plant in plantCollection)
        {
            SetPlantStatus(plant, false);
        }

        SetPlantStatus(plantCollection[0], true);
    }

    public void SetPlantStatus(GameObject plant, bool status)
    {
        if (plantStatus.ContainsKey(plant.name) != true)
        {
            plantStatus.Add(plant.name, status);
        }
        plantStatus[plant.name] = status;
            
    }

    public bool PlantActive(GameObject plant)
    {
        return plantStatus[plant.name];
    }

    public void RemovePlant(GameObject plant)
    {
        plantCollection.Remove(plant);
        plantStatus.Remove(plant.name);
    }

    // Update is called once per frame
    void Update()
    {
        float maxSpeed = 0.000001f;
        globalTimeSpeed = NumOp.Cutoff(globalTimeSpeed, maxSpeed, 1f);
        int i = 0;
        foreach (GameObject plant in plantCollection)
        {
            plant.GetComponent<Timer>().getTicks = PlantActive(plant);
            plant.GetComponent<Timer>().speed = globalTimeSpeed/maxSpeed;

            foreach (Timer timer in plant.GetComponentsInChildren<Timer>())
            {
                timer.getTicks = PlantActive(plant);
                timer.speed = globalTimeSpeed/maxSpeed;
            }
            if (PlantActive(plant))
            {
                //Place on lab space position according to index
            }
            i++;
        }
    }
}
