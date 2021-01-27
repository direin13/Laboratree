using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDeath : MonoBehaviour
{
    public GameObject[] leaves;
    // Start is called before the first frame update
    void Start()
    {
        PlantRates plantRates = transform.root.gameObject.GetComponent<PlantRates>();
        plantRates.TimeStamps().Set(name + " deathfade", 10, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        PlantRates plantRates = transform.root.gameObject.GetComponent<PlantRates>();
        if (plantRates.Health() == 0f)
        {
            print("Dead");
        }
    }
}
