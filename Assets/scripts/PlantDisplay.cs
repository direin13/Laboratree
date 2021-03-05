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
            plant = Instantiate(pManager.plantCollection[indexNum]);
            plant.SetActive(true);
            plant.transform.localScale = new Vector3(35, 35, 1);
            plant.transform.position = new Vector3(70, -112, -60);
            plant.BroadcastMessage("ReadGenesOnStart", false);
            plant.GetComponent<Timer>().getTicks = false;
        }
        return plant;
    }

    void Start()
    {
        prevIndexNum = -1;
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
        if (!currPlant)
        {
            if (nameText)
                nameText.text = "N/A";
            currPlant = makeClone();
        }

        else
        {
            currPlant.transform.position = new Vector3(followPoint.transform.position.x, followPoint.transform.position.y, followPoint.transform.position.z);
            currPlant.GetComponent<Timer>().timeElapsed = pManager.plantCollection[indexNum].GetComponent<Timer>().timeElapsed;
            if (nameText)
                nameText.text = pManager.plantCollection[indexNum].name;
            print(nameText.text);

            string[] dependencies = { "Lighting", "Water", "Temperature", "Fertiliser" };
            foreach (string dep in dependencies)
            {
                DependenceAttribute depCompOrigin = pManager.plantCollection[indexNum].GetComponent<PlantRates>().GetDepComp(dep);
                DependenceAttribute depCompClone = currPlant.GetComponent<PlantRates>().GetDepComp(dep);

                depCompClone.currValue = depCompOrigin.currValue;
            }
        }


        //switch to next/previous plant
        if (leftButton && leftButton.GetComponent<NavigateButtons>().clicked == true)
        {
            indexNum--;
            leftButton.GetComponent<NavigateButtons>().clicked = false;
        }

        if (rightButton && rightButton.GetComponent<NavigateButtons>().clicked == true)
        {
            indexNum++;
            rightButton.GetComponent<NavigateButtons>().clicked = false;
        }

        //print(prevIndexNum.ToString() + " and " + indexNum.ToString());
        if (prevIndexNum != indexNum)
        {
            navigate();
        }

        if (indexText)
            indexText.text = (indexNum + 1).ToString() + "/" + pManager.plantCollection.Count.ToString();

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