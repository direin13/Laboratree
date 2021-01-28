using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class StemGrow : MonoBehaviour
{
    //used to sprout sprites from a point across a given angle
    private GameObject stem;
    public Sprite sprite;
    public int leafCount;
    public int growthStages;
    private Vector3 spawnPoint;
    public float angle;
    public float sproutSize;
    public float rotationOffset;
    public bool invHeightSkew;
    public float heightOffset;
    public float heightOffsetPower;
    public Vector3 offsetSpawnPoint;
    public Vector3 leafScale;

    public bool debug;


    public Vector3 mulVec(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1[0] * vec2[0], vec1[1] * vec2[1], vec1[2] * vec2[2]);
    }


    // Start is called before the first frame update
    void Start()
    {
        stem = transform.parent.gameObject;
        SpriteRenderer stemSprite = stem.GetComponent<SpriteRenderer>();
        spawnPoint = new Vector3(stemSprite.transform.position[0],
                                 stemSprite.transform.position[1],
                                 stemSprite.transform.position[2] + 2) + mulVec(offsetSpawnPoint, transform.root.localScale);

    }

    public void SetLocalScale(GameObject obj, Vector3 scale)
    {
        obj.transform.localScale = scale;
    }

    public void SetLeavesRotation(float newAngle, float anglePower, float rotationOffset)
    {
        float maxAngle = newAngle * anglePower;
        //get angles on one side and mirror to the other side
        float zAngle = NumOp.Cutoff(maxAngle, 0, 360f);

        if (zAngle >= 0)
        {
            Vector3 rotation = new Vector3(0, 0, zAngle);

            transform.parent.rotation = Quaternion.Euler(rotation);
        }
    }

    public void SetHeightSkew(float skewAmount, float offsetAmount, bool reverseSkew, Vector3 leafScale)
    {
        //sets how much the leaves' height are skewed from the centre
        //offsetAmount represents the amount of skew used the further
        //the leaf is from the center
        //skewAmount and offSetAMount are floats from  0 -> 1

        Vector3 interval = leafScale - (leafScale * skewAmount);
        print(interval[1]);
        SetLocalScale(gameObject, interval);

        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
        }
    }

    public void SetColor()
    {
        Color chosenColor = GetComponent<TimeToColor>().realTimeColor;
        //color the fore and background components

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = chosenColor;
    }

    // Update is called once per frame
    void Update()
    {
        //set variables within appropriate ranges
        rotationOffset = NumOp.Cutoff(rotationOffset, 0f, 1f);
        sproutSize = NumOp.Cutoff(sproutSize, 0f, 1f);
        heightOffset = NumOp.Cutoff(heightOffset, 0f, 1f);
        angle = NumOp.Cutoff(angle, 0f, 360f);

        PlantRates plant = transform.root.gameObject.GetComponent<PlantRates>();

        float growthAmount = plant.GrowthAmount(growthStages);

        SetLeavesRotation(angle * growthAmount, sproutSize * growthAmount, rotationOffset * growthAmount);
        SetHeightSkew(heightOffset * growthAmount, heightOffsetPower * growthAmount, invHeightSkew, leafScale * growthAmount);
        SetColor();
    }
}
