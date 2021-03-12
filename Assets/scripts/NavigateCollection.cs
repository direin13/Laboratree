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

    public GameObject followPoint;
    public Transform parent;
    private GameObject currPlant;
    
    public int indexNum;
    public int prevIndexNum;
    private PlantManager pManager;
    public GameObject activateButton;

    float getCurrVal(string attribute) {
        return pManager.plantCollection[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;        //return current value of dependency depending on attribute
    }

    public GameObject makeClone(){
        GameObject plant = null;
        if (pManager.plantCollection.Count > 0)     //only when coolection is not empty 
        {
            plant = Instantiate(pManager.plantCollection[indexNum]);        //create visible prefab/clone of plant at index 
            plant.SetActive(true);      //make visible
            plant.transform.localScale = new Vector3(35, 35, 1);        //set scale
            plant.transform.position = new Vector3(70, -112, -60);      //set position
            plant.BroadcastMessage("ReadGenesOnStart", false);          //broadcast message to relevant components
            plant.GetComponent<Timer>().getTicks = false;               //stop plant from growing in relation to time
        }
        return plant;
    }

    void Start(){
        pManager = GameObject.Find("GameManager").GetComponent<PlantManager>();     
    }

    public void display(){

        currPlant = makeClone();    //set currently displayed plant to var

    }

    void Update()
    {

        if (!currPlant) //if no plant, set to n/a 
        {
            timeAlive.text = String.Format("Days Alive: {0}", "N/A");
            healthEfficiency.text = String.Format("Health: {0}", "N/A");
            nameText.text = "N/A";

            //attempt to make clone when new plant added to empty collection
            try {
                currPlant = makeClone();        
            } catch (ArgumentOutOfRangeException) {
                navigate();         //if attempt fails, adjust index value
            }

        }

        else    //if plant collection not empty, display plant and details
        {
            nameText.text = pManager.plantCollection[indexNum].name;

            var lighting = getCurrVal("Lighting").ToString() + " lumen(s)";
            var temp = getCurrVal("Temperature").ToString() + "°C";
            var water = getCurrVal("Water").ToString() + " day(s)";
            var fertiliser = getCurrVal("Fertiliser").ToString() + " day(s)";

            //set text in environmental dependency fields
            lightInput.text = lighting;
            tempInput.text = temp;
            waterInput.text = water;
            fertiliserInput.text = fertiliser;


            currPlant.transform.position = new Vector3(followPoint.transform.position.x, followPoint.transform.position.y, followPoint.transform.position.z);     //set position to follow point
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

                depCompClone.currValue = depCompOrigin.currValue;       //match dependencies between clone and origin

            }
        }

        if (prevIndexNum != indexNum)
        {
            navigate();     //adjust index
        }

        if (indexText)      //update value in index box
        {
            if (pManager.plantCollection.Count == 0)
                indexText.text = "0" + "/" + pManager.plantCollection.Count.ToString();
            else
                indexText.text = (indexNum + 1).ToString() + "/" + pManager.plantCollection.Count.ToString();
        }

        if (currPlant && activateButton)    //update plant's activated/deactivated status
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

        //remove previous plant clone
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

    public void ActivatePlant()
    {
        GameObject gameManager = pManager.gameObject;
        try     //adding plant to labspace 
        {
            pManager.SetPlantStatus(pManager.plantCollection[indexNum], true);
            gameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been added to the labspace", pManager.plantCollection[indexNum].name));
        }
        catch (ArgumentException) //no space or is active already
        {       
            if (pManager.PlantActive(pManager.plantCollection[indexNum]))       //remove from labspace
            {
                pManager.SetPlantStatus(pManager.plantCollection[indexNum], false);
                gameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been removed from the labspace", pManager.plantCollection[indexNum].name));
            }
            else    //if labspace already full, prompt to swap plants 
            {
                gameManager.GetComponent<PopUpManager>().SwapPlant(indexNum);           
            }
        }

        prevIndexNum = -1; //trigger new clone of plant
    }

    public void OnDisable()
    {
        Destroy(currPlant);     //destroy unneeded plant display
    }

    public void OnEnable()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        PlantManager plantManager = gameManager.GetComponent<PlantManager>();
        prevIndexNum = -1; //trigger a new clone
    }

    //to remove from collection
    public void deleteFromList(){
        string plantName = pManager.plantCollection[indexNum].name;

        //destory and reset currplant
        DestroyImmediate(currPlant);
        currPlant = null;
        pManager.RemovePlant(indexNum);     //remove from collection
        indexNum = NumOp.Cutoff(indexNum, 0, pManager.plantCollection.Count-1);     //set new index

        pManager.gameObject.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been deleted.", plantName));
        prevIndexNum = indexNum;
    }

}
