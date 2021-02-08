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
            print("changed to value: " + attributeObj.GetComponent<DependenceAttribute>().currValue.ToString());

            // change placeholder to current val
            placeholder.text = input.text;

            //reset values
            input.text = "";
            button.GetComponent<NavigateButtons>().clicked = false;
        }

    }
}
