using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CrossBreedClick : MonoBehaviour
{
    public GameObject plantDisplay1;
    public GameObject plantDisplay2;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void CrossBreed()
    {
        PlantManager plantManager = GameObject.Find("GameManager").GetComponent<PlantManager>();
        int index1 = plantDisplay1.GetComponent<PlantDisplay>().indexNum;
        int index2 = plantDisplay2.GetComponent<PlantDisplay>().indexNum;
        print(index1.ToString() + " " + index2.ToString());
        if (index1 == index2)
        {
            print("cannot crossbreed the same plant");
        }
        else if (plantManager.plantCollection.Count <= 1)
        {
            print("There aren't a sufficient number of plants in the collection");
        }
        else
        {

            plantManager.Breed(plantManager.plantCollection[index1], plantManager.plantCollection[index2], "New Breed");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
