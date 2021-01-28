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
        Populate();
    }

    void Populate()
    {
        GameObject newObj;  //creating new instance

        //loop to instantiate prefabs - currently instantiates only one type
        for (int i = 0; i < numberToCreate; i++)
        {
            //creates and positions prefabs
            newObj = (GameObject)Instantiate(prefab, new Vector3((i * 20.0F) + 10 , -100, -20), Quaternion.identity);
            newObj.GetComponent<Transform>().SetParent(GetComponent<Transform>());
            newObj.GetComponent<Transform>().localScale = new Vector3(30,30,1);

            newObj = (GameObject)Instantiate(prefab, new Vector3((i * 20.0F) + 10 , -100, -35), Quaternion.identity);
        }

    }

}