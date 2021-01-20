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

    void Update()
    {

    }

    void Populate()
    {
        GameObject newObj;  //creating new instance

        for (int i = 0; i < numberToCreate; i++)
        {
            newObj = (GameObject)Instantiate(prefab,transform);
        }

    }

}