using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{

    [SerializeField]
    private GameObject point, plant;

    void OnEnable(){
        if (plant)
            plant.SetActive(true);      //make plant visible to user
    }

    void Update(){
        if (plant)
            plant.transform.position = new Vector3(point.transform.position.x, point.transform.position.y,-20);        //place plant in same position as designated point
    }


    void OnDisable(){
        if (plant)
            plant.SetActive(false);     //make plant invisible to user
    }
}
