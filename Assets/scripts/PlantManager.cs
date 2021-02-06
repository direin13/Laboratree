using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;
using UnityEngine.UI;

public class PlantManager : MonoBehaviour
{
    //This is the script that manages all the plants in the game
    public GameObject timeSlider;
    private float globalTimeSpeed;
    public List<GameObject> plantCollection;
    public Vector3 plantStartPos;
    public Vector3 plantEndPos;
    public readonly Dictionary<string, bool> plantStatus = new Dictionary<string, bool>(); //used to see if plant is on labspace
    private int timeElapsed;
    private readonly int maxActivePlants = 3;

    public List<string> allPrefabs;
    private readonly Dictionary<string, GameObject> prefabMappings = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        //load all the given prefabs
        foreach (string prefab in allPrefabs)
        {
            GameObject plant = (GameObject)Resources.Load("plant prefabs/"+prefab);
            if (prefabMappings.ContainsKey(prefab))
            {
                prefabMappings.Remove(prefab);
                Debug.LogWarning(String.Format("Prefab '{0}' is already added. Replacing...", prefab), gameObject);
            }
            prefabMappings.Add(prefab, plant);
        }

        //set the plants to active/inactive
        foreach (GameObject plant in plantCollection)
        {
            SetPlantStatus(plant, false);
        }

        plantStatus[plantCollection[0].name] = true;
        GetComponent<Timer>().getTicks = true;
    }

    public void SetPlantStatus(GameObject plant, bool status)
    {
        //set status of plant. If true, plant will be shown in labspace
        //plant must be in the plantCollection or exception will be thrown
        //shouldnt be 3 or more plants active if setting to true

        int numberActive = 0;
        bool inCollection = false;
        foreach (GameObject p in plantCollection)
        {
            try
            {
                if (p != plant && PlantActive(p))
                {
                    numberActive++;
                }
            }
            catch (KeyNotFoundException) //if new plant was being added
            { }

            if (p == plant)
            {
                inCollection = true;
            }
        }

        if (numberActive >= maxActivePlants && status)
        {
            throw new Exception("Active plants are greater than the quota. Must deactivate already active plant before adding new ones!");
        }

        if (!inCollection)
        {
            throw new Exception(String.Format("'{0}' is not in the plant collection!", plant.name));
        }

        //set status
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

    public GameObject MakePlant(string name)
    {
        foreach(GameObject p in plantCollection)
        {
            if (p.name == name)
            {
                throw new Exception(String.Format("'{0}' is already in the plant collection!", name));
            }
        }
        //Loads basic plant object
        GameObject plant = GameObject.Instantiate(prefabMappings["Basic Plant"]);
        plant.name = name;
        plantStatus[plant.name] = false;
        plantCollection.Add(plant);
        return plant;
    }

    public GameObject MakePlant(string name, string prefab)
    {
        foreach (GameObject p in plantCollection)
        {
            if (p.name == name)
            {
                throw new Exception(String.Format("'{0}' is already in the plant collection!", name));
            }
        }
        //Loads plant object from given plant prefab
        GameObject plant = GameObject.Instantiate(prefabMappings[prefab]);
        plant.name = name;
        plantStatus[plant.name] = false;
        plantCollection.Add(plant);
        return plant;
    }

    // Update is called once per frame
    void Update()
    {
        float maxSpeed = 0.01f;
        globalTimeSpeed = NumOp.Cutoff(1f - timeSlider.GetComponent<Slider>().value, maxSpeed, 1f);

        GetComponent<Timer>().speed = globalTimeSpeed / maxSpeed;

        int i = 0;
        foreach (GameObject plant in plantCollection)
        {
            plant.GetComponent<Timer>().getTicks = PlantActive(plant);
            plant.GetComponent<Timer>().speed = GetComponent<Timer>().speed;

            foreach (Timer timer in plant.GetComponentsInChildren<Timer>())
            {
                timer.getTicks = PlantActive(plant);
                timer.speed = GetComponent<Timer>().speed;
            }

            if (PlantActive(plant))
            {
                //Place on lab space position according to index
                plant.SetActive(true);
                if (i == 0)
                {
                    plant.transform.position = plantStartPos;
                }
                else if (i > 0 && i < maxActivePlants-1)
                {
                    float inBetween = 0.5f * (plantEndPos[0] - plantStartPos[0]);
                    plant.transform.position = new Vector3(plantStartPos[0] + inBetween, plantStartPos[1], plantStartPos[2]);
                }
                else if (i == maxActivePlants-1)
                {
                    plant.transform.position = plantEndPos;
                }
                i++;
            }

            else
            {
                //deactivate plant
                plant.SetActive(false);
            }
        }

        if (GetComponent<Timer>().Tick())
        {
            timeElapsed++;
        }
        
        if (timeElapsed == 10)
        {
          SetPlantStatus(MakePlant("joes plant", "Aloe"), true);
        }
    }
}
