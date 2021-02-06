using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using System;


public class DisplayPlants : MonoBehaviour
{
    public GameObject prefab;   //plant
    public int numberToCreate;

    void Start()
    {
        //Populate();
    }

    void Populate()
    {
        GameObject newObj;  //creating new instance

        //loop to instantiate prefabs - currently instantiates only one type

        //creates and positions prefabs
        float x = 40f;
        float y = -200f;
        newObj = (GameObject)Instantiate(prefab);
        newObj.transform.position = new Vector3(x, y, -35);
        newObj.GetComponent<Transform>().SetParent(GetComponent<Transform>());
        newObj.GetComponent<Transform>().localScale = new Vector3(30,30,1);

        //trying to fix issue with aloe not displaying correctly with grid layout component
        // newObj.transform.position = new Vector3(newObj.transform.position.x,newObj.transform.position.y, 0);
        // DontDestroyOnLoad(newObj);
    }

}