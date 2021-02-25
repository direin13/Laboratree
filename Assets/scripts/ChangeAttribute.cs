using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeAttribute : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI placeholder;
    public TMP_InputField input;

    //shallow copies
    private List<GameObject> plantList;
    private int indexNum;

    void Start(){

        plantList = GameObject.Find("PlantCollectionPage").GetComponent<NavigateCollection>().plantList;
    }

    void Update() {
        //if apply button clicked
        if (button.GetComponent<NavigateButtons>().clicked == true) {

            indexNum = GameObject.Find("PlantCollectionPage").GetComponent<NavigateCollection>().indexNum;
            // print("index num is " + indexNum.ToString());
            print("Apply button clicked");

            //name of attribute to be changed
            string attribute = this.name;
            
            //finding dependency object to change in current plant
            Transform dependencyObj = plantList[indexNum].transform.Find("Dependencies");
            Transform attributeObj = dependencyObj.transform.Find(attribute);

            //set current val to new val
            attributeObj.GetComponent<DependenceAttribute>().currValue = float.Parse(input.text);

            string measurement = "";

            //get measurement value
            if (this.name == "Lighting") {
                measurement = " lumen(s)";
            }
            if (this.name == "Temperature") {
                measurement = "°C";
            }
            if (this.name == "Water" || this.name == "Fertiliser" ) {
                measurement = " day(s)";
            }
            // change placeholder to current val
            placeholder.text = input.text + measurement;

            //reset values
            input.text = "";
            button.GetComponent<NavigateButtons>().clicked = false;
        }

    }
}
