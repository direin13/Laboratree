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
    private TextMeshProUGUI nameText,lightInput,tempInput,waterInput,fertiliserInput;

    public GameObject FollowPoint;
    public Transform parent;
    public List<GameObject> plantList;
    private GameObject currPlant;
    
    public int indexNum;
    public int prevIndexNum;

    float getCurrVal(string attribute) {
        return plantList[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    GameObject makeClone(){
        
        GameObject plant = Instantiate(plantList[indexNum]);
        plant.SetActive(true);
        plant.transform.localScale = new Vector3(35,35,1);
        plant.transform.position = new Vector3(70,-112,-60);
        plant.BroadcastMessage("ReadGenesOnStart", false);
        return plant;
    }

    void Start(){
    }

    void display(){

        currPlant = makeClone();

        //change name to current plant
        nameText.text = "Name: " + plantList[indexNum].name;

        //change attribute values
        var lighting = getCurrVal("Lighting").ToString();
        var temp = getCurrVal("Temperature").ToString();
        var water = getCurrVal("Water").ToString();
        var fertiliser = getCurrVal("Fertiliser").ToString();

        //set text in interval fields
        lightInput.text = lighting;
        tempInput.text = temp;
        waterInput.text = water;
        fertiliserInput.text = fertiliser;

    }

    void Update()
    {


        if (plantList.Count <= 0)
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
            currPlant.GetComponent<Timer>().timeElapsed = plantList[indexNum].GetComponent<Timer>().timeElapsed;
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
            indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();
    }

    //displays and navigates between plants in list
    void navigate()
    {
        if (indexNum >= plantList.Count)
        {
            indexNum = 0;
        }
        else if (indexNum < 0)
        {
            indexNum = plantList.Count - 1;
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
        indexNum = 0;   //starts at 0
        prevIndexNum = indexNum;
    }
}
