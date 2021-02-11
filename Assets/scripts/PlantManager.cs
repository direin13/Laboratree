using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using UnityEditor;

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

    private readonly static string prefabPath = "plant prefabs/";
    public List<string> allPrefabs;
    private readonly Dictionary<string, GameObject> prefabMappings = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        //load all the given prefabs
        foreach (string prefab in allPrefabs)
        {
            GameObject plant = (GameObject)Resources.Load(prefabPath+prefab);
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

        //testing manually set first plant active, will remove later
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
            throw new ArgumentException("Active plants are greater than the quota. Must deactivate already active plant before adding new ones!");
        }

        if (!inCollection)
        {
            throw new ArgumentException(String.Format("'{0}' is not in the plant collection!", plant.name));
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
        //Load and instantiate basic plant object
        foreach(GameObject p in plantCollection)
        {
            if (p.name == name)
            {
                throw new Exception(String.Format("'{0}' is already in the plant collection!", name));
            }
        }

        GameObject plant = GameObject.Instantiate(prefabMappings["Basic Plant"]);
        plant.name = name;
        plantStatus[plant.name] = false;
        plantCollection.Add(plant);
        return plant;
    }


    public GameObject MakePlant(string name, string prefab)
    {
        //Load and instantiates plant object from given plant prefab
        foreach (GameObject p in plantCollection)
        {
            if (p.name == name)
            {
                throw new Exception(String.Format("'{0}' is already in the plant collection!", name));
            }
        }

        GameObject plant = GameObject.Instantiate(prefabMappings[prefab]);
        plant.name = name;
        plantStatus[plant.name] = false;
        plantCollection.Add(plant);
        return plant;
    }

    // Update is called once per frame
    void Update()
    {
        //setting the timer speed for every plant in the collection
        float maxSpeed = GetComponent<Timer>().maxSpeed;
        globalTimeSpeed = NumOp.Cutoff(1f - timeSlider.GetComponent<Slider>().value, maxSpeed, 1f);

        GetComponent<Timer>().speed = globalTimeSpeed / maxSpeed;

        int i = 0;
        foreach (GameObject plant in plantCollection)
        {

            plant.GetComponent<Timer>().getTicks = PlantActive(plant);
            plant.GetComponent<Timer>().speed = GetComponent<Timer>().speed;
            

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
        

        //testing making some plants dynamically
        if (timeElapsed == 10)
        {
          SetPlantStatus(MakePlant("joes plant", "Jade"), true);
        }


        if (timeElapsed == 100)
        {
            SetPlantStatus(Breed(plantCollection[0], plantCollection[1], "AloexJoe"), true);
        }
    }


    public GameObject Breed(GameObject plant1, GameObject plant2, string outName)
    {
        //breed/crossbreeds 2 plants and returns new plant of name outName


        GameObject outPlant = MakePlant(outName);
        //mix life expectancy of plant
        plant1.GetComponent<Genes>().CrossGenes(plant2.GetComponent<Genes>(), outPlant.GetComponent<Genes>());

        string[] dependencyNames = { "Lighting", "Temperature", "Water", "Fertiliser" };
        foreach (string dep in dependencyNames)
        {
            Genes dep1Gene = plant1.GetComponent<PlantRates>().GetDepComp(dep).gameObject.GetComponent<Genes>();
            Genes dep2Gene = plant2.GetComponent<PlantRates>().GetDepComp(dep).gameObject.GetComponent<Genes>();
            Genes outGene = outPlant.GetComponent<PlantRates>().GetDepComp(dep).gameObject.GetComponent<Genes>();

            dep1Gene.CrossGenes(dep1Gene, outGene);
        }

        //choose structure passed down
        GameObject plant1Struct = plant1.transform.Find("Structure").gameObject;
        GameObject plant2Struct = plant2.transform.Find("Structure").gameObject;
        Destroy(outPlant.transform.Find("Structure").gameObject);

        GameObject childStruct;
        GameObject unchosenStruct;
        int rInt = Random.Range(0, 2);
        if (rInt == 0)
        {
            childStruct = (GameObject)Instantiate(plant1Struct, outPlant.transform);
            unchosenStruct = plant2Struct;
        }
        else
        {
            childStruct = (GameObject)Instantiate(plant2Struct, outPlant.transform);
            unchosenStruct = plant1Struct;
        }
        childStruct.name = "Structure";
        outPlant.GetComponent<Timer>().timeElapsed = 0;


        //mix unchosen structure into child structure
        //get every obj in chosen structure and mix it with
        //obj of the same type in unchosen structure
        GameObject[] allChildStructObj = getSubObjects(childStruct);
        GameObject[] allParentStructObj = getSubObjects(unchosenStruct);

        foreach (GameObject obj in allChildStructObj)
        {
            Genes objGenes = obj.GetComponent<Genes>();
            if (objGenes != null)
            {
                foreach (GameObject otherObj in allParentStructObj)
                {
                    print(obj.name + " " + otherObj.name);
                    print(otherObj.GetComponent<Genes>());
                    if (otherObj.name == obj.name)
                    {
                        print("mixing");
                        objGenes.CrossGenes(otherObj.GetComponent<Genes>(), objGenes);
                    }
                }
            }
        }

        return outPlant;
    }

    public GameObject[] getSubObjects(GameObject obj)
    {
        Transform[] allTrans = obj.GetComponentsInChildren<Transform>();
        GameObject[] out_ = new GameObject[allTrans.Length-1];
        for (int i = 1; i < allTrans.Length; i++)
        {
            GameObject gObj = allTrans[i].gameObject;
            out_[i-1] = gObj;
        }
        return out_;
    }
}
