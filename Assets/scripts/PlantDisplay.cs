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


    float getCurrVal(string attribute)
    {
        return plantList[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    GameObject makeClone()
    {
        GameObject plant = Instantiate(pManager.plantCollection[indexNum]);
        plant.SetActive(true);
        plant.transform.localScale = plantScale;
        plant.transform.position = new Vector3(0, -112, -60);
        plant.GetComponent<Timer>().getTicks = false;
        plant.BroadcastMessage("ReadGenesOnStart", false);
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
        print(index.ToString() + prevIndexNum.ToString());
    }

    void display()
    {
        currPlant = makeClone();
        //change name to current plant
        if (nameText)
            nameText.text = pManager.plantCollection[indexNum].name;
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
            currPlant.transform.position = new Vector3(followPoint.transform.position.x, followPoint.transform.position.y, followPoint.transform.position.z);
            currPlant.GetComponent<Timer>().timeElapsed = pManager.plantCollection[indexNum].GetComponent<Timer>().timeElapsed;
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
            print("ok");
            navigate();
        }

        if (indexText)
            indexText.text = (indexNum + 1).ToString() + "/" + pManager.plantCollection.Count.ToString();

    }

    //displays and navigates between plants in list
    void navigate()
    {
        print("navigating");
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