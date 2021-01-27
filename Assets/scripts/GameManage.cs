using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public float globalTimeSpeedPercentage;
    public List<GameObject> plantCollection;
    public List<GameObject> inLabSpace;
    // Start is called before the first frame update
    void Start()
    {
        plantCollection = new List<GameObject>();
        inLabSpace = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
