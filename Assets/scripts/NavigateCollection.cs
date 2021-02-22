using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class NavigateCollection : MonoBehaviour
{
    public TextMeshProUGUI indexText;

    [SerializeField]
    private Button leftButton, rightButton;
    
    [SerializeField]
    private TextMeshProUGUI nameText,lightInput,tempInput,waterInput,fertiliserInput,healthEfficiency,timeAlive;

    public GameObject FollowPoint;
    public Transform parent;
    public List<GameObject> plantList;
    private GameObject currPlant;
    
    public int indexNum;
    public int prevIndexNum;
    private PlantManager pManager;
    public GameObject activateButton;

    float getCurrVal(string attribute) {
        return plantList[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    GameObject makeClone(){
        
        GameObject plant = Instantiate(pManager.plantCollection[indexNum]);
        plant.SetActive(true);
        plant.transform.localScale = new Vector3(35,35,1);
        plant.transform.position = new Vector3(70,-112,-60);
        plant.BroadcastMessage("ReadGenesOnStart", false);
        plant.GetComponent<Timer>().getTicks = false;
        return plant;
    }

    void Start(){
        pManager = GameObject.Find("GameManager").GetComponent<PlantManager>();
    }

    void display(){

        currPlant = makeClone();

        //change name to current plant
        nameText.text = pManager.plantCollection[indexNum].name;

        //change attribute values
        var lighting = getCurrVal("Lighting").ToString() + " lumen(s)";
        var temp = getCurrVal("Temperature").ToString() + "°C";
        var water = getCurrVal("Water").ToString() + " day(s)";
        var fertiliser = getCurrVal("Fertiliser").ToString() + " day(s)";

        //set text in interval fields
        lightInput.text = lighting;
        tempInput.text = temp;
        waterInput.text = water;
        fertiliserInput.text = fertiliser;
    }

    void Update()
    {


        if (pManager.plantCollection.Count <= 0)
        {
            currPlant = null;
        }

        else if (!currPlant)
        {
            display();
        }

        if (currPlant)
        {
            currPlant.transform.position = new Vector3(FollowPoint.transform.position.x, FollowPoint.transform.position.y, currPlant.transform.position.z);
            currPlant.GetComponent<Timer>().timeElapsed = plantList[indexNum].GetComponent<Timer>().timeElapsed;        ///time alive
            var numDays = plantList[indexNum].GetComponent<Timer>().timeElapsed / 24;
            timeAlive.text = String.Format("Days Alive: {0}", numDays);
            healthEfficiency.text = String.Format("Health Efficiency: {0:0.0000}", plantList[indexNum].GetComponent<PlantRates>().currEfficiency); //health efficiency
        }

        //switch to next/previous plant
        if (leftButton.GetComponent<NavigateButtons>().clicked == true)
        {
            indexNum--;
            leftButton.GetComponent<NavigateButtons>().clicked = false;
        }

        if (rightButton.GetComponent<NavigateButtons>().clicked == true)
        {
            indexNum++;
            rightButton.GetComponent<NavigateButtons>().clicked = false;
        }

        if (prevIndexNum != indexNum)
        {
            navigate();
        }

        if (indexText)
        {
            if (pManager.plantCollection.Count == 0)
                indexText.text = (indexNum).ToString() + "/" + pManager.plantCollection.Count.ToString();
            else
                indexText.text = (indexNum + 1).ToString() + "/" + pManager.plantCollection.Count.ToString();
        }

        if (currPlant && activateButton)
        {
            TMP_Text buttonText = activateButton.GetComponentInChildren<TMP_Text>();
            if ( pManager.PlantActive(pManager.plantCollection[indexNum]) )
            {
                buttonText.text = "Deactivate";
            }
            else
            {
                buttonText.text = "Activate";
            }
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

    public void ActivatePlant()
    {
        GameObject gameManager = pManager.gameObject;
        try
        {
            pManager.SetPlantStatus(pManager.plantCollection[indexNum], true);
            gameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been added to the labspace", pManager.plantCollection[indexNum].name));
        }
        catch (ArgumentException) //no space or is active already
        {       
            if (pManager.PlantActive(pManager.plantCollection[indexNum]))
            {
                pManager.SetPlantStatus(pManager.plantCollection[indexNum], false);
                gameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been removed from the labspace", pManager.plantCollection[indexNum].name));
            }
            else
            {
                gameManager.GetComponent<PopUpManager>().SwapPlant(indexNum);
            }
        }
    }

    public void OnDisable()
    {
        Destroy(currPlant);
    }

    public void OnEnable()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        PlantManager plantManager = gameManager.GetComponent<PlantManager>();
        plantList = plantManager.plantCollection;
        indexNum = 0;   //starts at 0
        prevIndexNum = -1;
    }

    public void deleteFromList(){
        string plantName = pManager.plantCollection[indexNum].name;
        pManager.RemovePlant(indexNum);
        Destroy(currPlant);
        currPlant = null;
        //trigger next plant using right button
        rightButton.GetComponent<NavigateButtons>().clicked = true;
        if (indexNum - 1 == pManager.plantCollection.Count - 1)
        {
            indexNum--;
        }

        pManager.gameObject.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been deleted.", plantName));
    }

}
