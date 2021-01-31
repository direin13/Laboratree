using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class NavigateCollection : MonoBehaviour
{
    public List<GameObject> plantCollection = PlantManager.plantCollection;

    void Start(){
        print(plantCollection[1]);
    }

}
