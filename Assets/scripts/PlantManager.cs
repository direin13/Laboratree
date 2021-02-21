using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using UnityEditor;
using System.Reflection;
using System.Linq;


public class PlantManager : MonoBehaviour
{
    //This is the script that manages all the plants in the game
    public GameObject timeSlider;
    private float globalTimeSpeed;
    public List<GameObject> plantCollection;
    public GameObject[] activePlants;
    public Vector3 plantStartPos;
    public Vector3 plantEndPos;
    private int timeElapsed;
    private readonly int maxActivePlants = 3;

    private readonly static string prefabPath = "plant prefabs/";
    public List<string> allPrefabs;
    private readonly Dictionary<string, GameObject> prefabMappings = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        activePlants = new GameObject[maxActivePlants];
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
        GetComponent<Timer>().getTicks = true;
        GameObject testPlant = MakePlant("Echeveria", "Echeveria");
        SetPlantStatus(testPlant, true);
    }

    public void SetPlantStatus(GameObject plant, bool status)
    {
        //set status of plant. If true, plant will be shown in labspace
        //plant must be in the plantCollection or exception will be thrown
        //shouldnt be 3 or more plants active if setting to true

        int numberActive = 0;
        foreach (GameObject p in activePlants)
        {
            try
            {
                if (p)
                {
                    numberActive++;
                }
            }
            catch (KeyNotFoundException) //if new plant was being added
            { }
        }

        if (numberActive >= maxActivePlants && status)
        {
            throw new ArgumentException("Active plants are greater than the quota. Must deactivate already active plant before adding new ones!");
        }

        if (activePlants.Contains(plant) && status)
        {
            throw new ArgumentException(String.Format("'{0}' is already in the collection!", plant.name));
        }


        int i = 0;
        bool spaceFound = false;
        while (i < activePlants.Length && !spaceFound)
        {
            if (status)
            {
                //automatically put in labspace if there's space
                if (activePlants[i] == null)
                {
                    spaceFound = true;
                    activePlants[i] = plant;

                    if (i == 0)
                    {
                        plant.transform.position = plantStartPos;
                    }
                    else if (i > 0 && i < maxActivePlants - 1)
                    {
                        float inBetween = 0.5f * (plantEndPos[0] - plantStartPos[0]);
                        plant.transform.position = new Vector3(plantStartPos[0] + inBetween, plantStartPos[1], plantStartPos[2]);
                    }
                    else if (i == maxActivePlants - 1)
                    {
                        plant.transform.position = plantEndPos;
                    }
                }
            }
            else
            {
                if (activePlants[i] == plant)
                {
                    activePlants[i] = null;
                }

            }
            i++;
        }

        if (status)
        {
            plant.GetComponent<Timer>().getTicks = true;
            plant.SetActive(true);
        }
        else
        {
            plant.GetComponent<Timer>().getTicks = false;
            plant.SetActive(false);
            plant.transform.position = new Vector3(-1000, -1000, 1);
        }
    }


    public bool PlantActive(GameObject plant)
    {
        return activePlants.Contains(plant);
    }


    public void RemovePlant(GameObject plant)
    {
        SetPlantStatus(plant, false);
        plantCollection.Remove(plant);
    }

    public void RemovePlant(int i)
    {
        SetPlantStatus(plantCollection[i], false);
        plantCollection.Remove(plantCollection[i]);
    }

    public int [] GetActivePlantIndexes()
    {
        int[] indexes = new int[maxActivePlants];
        int j = 0;

        foreach (GameObject plant in activePlants)
        {
            if (plant)
            {
                indexes[j] = plantCollection.IndexOf(plant);
                j++;
            }
        }
        return indexes;
    }

    public void SwapPlant(int i, int activePosition)
    {
        int replaced = Array.IndexOf(activePlants, plantCollection[activePosition]); 
        SetPlantStatus(activePlants[replaced], false);
        SetPlantStatus(plantCollection[i], true);
    }


    public GameObject MakePlant(string name, string prefab)
    {
        //Load and instantiates plant object from given plant prefab
        foreach (GameObject p in plantCollection)
        {
            if (p.name == name)
            {
                throw new ArgumentException(String.Format("'{0}' is already in the plant collection!", name));
            }
        }

        GameObject plant = GameObject.Instantiate(prefabMappings[prefab]);
        plant.name = name;
        plantCollection.Add(plant);
        SetPlantStatus(plant, false);

        ResetPlantComp(plant);

        return plant;
    }


    // Update is called once per frame
    void Update()
    {
        //setting the timer speed for every plant in the collection
        float maxSpeed = GetComponent<Timer>().maxSpeed;
        globalTimeSpeed = NumOp.Cutoff(1f - timeSlider.GetComponent<Slider>().value, maxSpeed, 1f);

        GetComponent<Timer>().speed = globalTimeSpeed / maxSpeed;

        foreach (GameObject plant in plantCollection)
        {
            plant.GetComponent<Timer>().speed = GetComponent<Timer>().speed;
        }


        if (GetComponent<Timer>().Tick())
        {
            timeElapsed++;
        }
        

        //testing making some plants dynamically
        if (timeElapsed == 10)
        {
          SetPlantStatus(MakePlant("joes plant", "Aloe"), true);
        }


        if (timeElapsed == 3000)
        {
            Breed(plantCollection[0], plantCollection[1], "AloexJoe");
        }

        if (timeElapsed == 3300)
        {
            SetPlantStatus(plantCollection[2], true);
            print("done");
        }
    }


    public GameObject Breed(GameObject plant1, GameObject plant2, string outName)
    {
        //breed/crossbreeds 2 plants and returns new plant of name outName


        GameObject outPlant = MakePlant(outName, "Aloe");
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
                    if (otherObj.name == obj.name)
                    {
                        objGenes.CrossGenes(otherObj.GetComponent<Genes>(), objGenes);
                    }
                }
            }
        }

        outPlant.GetComponent<Timer>().timeElapsed = 0;
        ResetPlantComp(outPlant);

        print("got through");

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

    public void ResetPlantComp(GameObject plant)
    {
        foreach (Sprout comp in plant.GetComponentsInChildren<Sprout>())
        {
            comp.ReadGenesOnStart(true);
        }
        foreach (Grow comp in plant.GetComponentsInChildren<Grow>())
        {
            comp.ReadGenesOnStart(true);
        }
        foreach (DependenceAttribute comp in plant.GetComponentsInChildren<DependenceAttribute>())
        {
            comp.ReadGenesOnStart(true);
        }
        foreach (PlantRates comp in plant.GetComponentsInChildren<PlantRates>())
        {
            comp.ReadGenesOnStart(true);
        }
        foreach (TimeToColor comp in plant.GetComponentsInChildren<TimeToColor>())
        {
            comp.ReadGenesOnStart(true);
        }
    }
}
