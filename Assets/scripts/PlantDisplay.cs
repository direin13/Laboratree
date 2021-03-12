using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PlantDisplay : MonoBehaviour
{
    public TextMeshProUGUI indexText;

    [SerializeField]
    private Button leftButton, rightButton;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private GameObject followPoint;

    public List<GameObject> plantList;
    private GameObject currPlant;

    private int prevIndexNum;
    public int indexNum;
    public Vector3 plantScale;
    private PlantManager pManager;

    public GameObject makeClone()
    {
        GameObject plant = null;
        if (pManager.plantCollection.Count > 0)
        {
            //create clone of original plant at index in collection
            plant = Instantiate(pManager.plantCollection[indexNum]);
            plant.SetActive(true);      //make visible
            plant.transform.localScale = new Vector3(35, 35, 1);
            plant.transform.position = new Vector3(70, -112, -60);
            plant.BroadcastMessage("ReadGenesOnStart", false);
            plant.GetComponent<Timer>().getTicks = false;
        }
        return plant;
    }

    void Start()
    {
        prevIndexNum = -1;      //trigger clone
        pManager = GameObject.Find("GameManager").GetComponent<PlantManager>();
    }

    public void ChangeIndex(int index)
    {
        //triggers a new clone in display
        prevIndexNum = -1;
        indexNum = index;
    }

    void Update()
    {

        if (!currPlant) //if no plant, set to n/a 
        {
            if (nameText) 
                nameText.text = "N/A";

            //attempt to make clone when new plant added to empty collection
            try
            {
                currPlant = makeClone();
            }
            catch (ArgumentOutOfRangeException)
            {
                navigate();         //if attempt fails, adjust index value
            }

        }

        else    //if plant collection not empty, display plant and details
        {
            if (nameText)
                nameText.text = pManager.plantCollection[indexNum].name;

            currPlant.transform.position = new Vector3(followPoint.transform.position.x, followPoint.transform.position.y, followPoint.transform.position.z);     //set position to follow point
            currPlant.GetComponent<Timer>().timeElapsed = pManager.plantCollection[indexNum].GetComponent<Timer>().timeElapsed;        ///time alive

            //update dependencies of clone
            string[] dependencies = { "Lighting", "Water", "Temperature", "Fertiliser" };
            foreach (string dep in dependencies)
            {
                DependenceAttribute depCompOrigin = pManager.plantCollection[indexNum].GetComponent<PlantRates>().GetDepComp(dep);
                DependenceAttribute depCompClone = currPlant.GetComponent<PlantRates>().GetDepComp(dep);

                depCompClone.currValue = depCompOrigin.currValue;       //match dependencies between clone and origin

            }
        }

        if (prevIndexNum != indexNum)
        {
            navigate();     //adjust index
        }

    }

    //displays and navigates between plants in list
    void navigate()
    {
        if (indexNum >= pManager.plantCollection.Count)
        {
            indexNum = 0;
        }
        else if (indexNum < 0)
        {
            indexNum = pManager.plantCollection.Count - 1;
        }

        Destroy(currPlant);
        currPlant = null;
        prevIndexNum = indexNum;
    }

    public void navigate(int indexIncrement)
    {
        indexNum = indexNum + indexIncrement;
        if (indexNum >= pManager.plantCollection.Count)
        {
            indexNum = 0;
        }
        else if (indexNum < 0)
        {
            indexNum = pManager.plantCollection.Count - 1;
        }

        //remove previous plant clone
        Destroy(currPlant);
        currPlant = null;

        prevIndexNum = indexNum;
    }


    public void OnDisable()
    {
        Destroy(currPlant);
    }

    public void OnEnable()
    {
        GameObject plantCollectionHolder = GameObject.Find("GameManager");
        PlantManager plantManager = plantCollectionHolder.GetComponent<PlantManager>();
        plantList = plantManager.plantCollection;
    }
}