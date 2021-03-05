using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using MiscFunctions;

public class NavigateCollection : MonoBehaviour
{
    public TextMeshProUGUI indexText;

    [SerializeField]
    private Button leftButton, rightButton;
    
    [SerializeField]
    private TextMeshProUGUI nameText,lightInput,tempInput,waterInput,fertiliserInput,healthEfficiency,timeAlive;

    public GameObject FollowPoint;
    public Transform parent;
    private GameObject currPlant;
    
    public int indexNum;
    public int prevIndexNum;
    private PlantManager pManager;
    public GameObject activateButton;

    float getCurrVal(string attribute) {
        return pManager.plantCollection[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    public GameObject makeClone(){
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

    void Start(){
        pManager = GameObject.Find("GameManager").GetComponent<PlantManager>();
    }

    public void display(){

        currPlant = makeClone();

    }

    void Update()
    {

        if (!currPlant)
        {
            timeAlive.text = String.Format("Days Alive: {0}", "N/A");
            healthEfficiency.text = String.Format("Health: {0}", "N/A"); //health efficiency
            nameText.text = "N/A";

            try {
                currPlant = makeClone();
            } catch (ArgumentOutOfRangeException) {
                navigate();
            }

        }

        else
        {
            nameText.text = pManager.plantCollection[indexNum].name;

            var lighting = getCurrVal("Lighting").ToString() + " lumen(s)";
            var temp = getCurrVal("Temperature").ToString() + "°C";
            var water = getCurrVal("Water").ToString() + " day(s)";
            var fertiliser = getCurrVal("Fertiliser").ToString() + " day(s)";

            //set text in interval fields
            lightInput.text = lighting;
            tempInput.text = temp;
            waterInput.text = water;
            fertiliserInput.text = fertiliser;


            currPlant.transform.position = new Vector3(FollowPoint.transform.position.x, FollowPoint.transform.position.y, currPlant.transform.position.z);
            currPlant.GetComponent<Timer>().timeElapsed = pManager.plantCollection[indexNum].GetComponent<Timer>().timeElapsed;        ///time alive
            var numDays = pManager.plantCollection[indexNum].GetComponent<Timer>().timeElapsed;
            timeAlive.text = String.Format("Days Alive: {0}", numDays);
            healthEfficiency.text = String.Format("Health: {0:0.00}%", pManager.plantCollection[indexNum].GetComponent<PlantRates>().currEfficiency * 100); //health efficiency

            //update dependencies of clone
            string[] dependencies = { "Lighting", "Water", "Temperature", "Fertiliser" };
            foreach (string dep in dependencies)
            {
                DependenceAttribute depCompOrigin = pManager.plantCollection[indexNum].GetComponent<PlantRates>().GetDepComp(dep);
                DependenceAttribute depCompClone = currPlant.GetComponent<PlantRates>().GetDepComp(dep);

                depCompClone.currValue = depCompOrigin.currValue;

            }
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
                indexText.text = "0" + "/" + pManager.plantCollection.Count.ToString();
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
    public void navigate()
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

        prevIndexNum = -1; //trigger new clone of plant
    }

    public void OnDisable()
    {
        Destroy(currPlant);
    }

    public void OnEnable()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        PlantManager plantManager = gameManager.GetComponent<PlantManager>();
        prevIndexNum = -1; //trigger a new clone
    }

    public void deleteFromList(){
        string plantName = pManager.plantCollection[indexNum].name;
        DestroyImmediate(currPlant);
        currPlant = null;
        pManager.RemovePlant(indexNum);
        indexNum = NumOp.Cutoff(indexNum, 0, pManager.plantCollection.Count-1);

        pManager.gameObject.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been deleted.", plantName));
        prevIndexNum = indexNum;
    }

}
