using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class Sprout : MonoBehaviour
{
    //used to sprout sprites from a point across a given angle
    private GameObject stem;
    public Sprite sprite;
    public int leafCount;
    private Vector3 spawnPoint;
    public float angle;
    public float rotationOffset;
    public float sproutSize;
    public bool invHeightSkew;
    public float heightOffset;
    public float heightOffsetPower;
    public Vector3 offsetSpawnPoint;
    public Vector3 leafScale;
    private GameObject[] leaves;

    public bool debug;


    public Vector3 mulVec(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1[0] * vec2[0], vec1[1] * vec2[1], vec1[2] * vec2[2]);
    }

    public GameObject[] CreateLeaves(int amount)
    {
        if (leaves != null)
        {
            foreach (GameObject leaf in leaves)
                Destroy(leaf.transform.parent.gameObject); //LeafNode
        }

        SpriteRenderer stemSprite = stem.GetComponent<SpriteRenderer>();
        spawnPoint = new Vector3(stemSprite.transform.position[0],
                                 stemSprite.transform.position[1],
                                 stemSprite.transform.position[2] + 2) + mulVec(offsetSpawnPoint, transform.root.localScale);

        GameObject[] newLeaves = new GameObject[amount];

        //creating each leaf
        for (int i = 0; i < amount; i++)
        {
            GameObject leaf = new GameObject(name + "' child " + i.ToString());
            newLeaves[i] = leaf;

            leaf.AddComponent<SpriteRenderer>();
            SpriteRenderer leafSpriteRenderer = leaf.GetComponent<SpriteRenderer>();
            leafSpriteRenderer.sprite = sprite;

            //setting the scale and default position of each leaf
            //each leaf has a node (not visible) at the bottom of the sprite.
            //Use this node to move/rotate leaf around an axis
            GameObject leafNode = new GameObject(leaf.name + " node");
            leafNode.transform.parent = gameObject.transform;
            leaf.transform.parent = leafNode.transform;

            float stemHeight = stemSprite.bounds.size.y;
            leaf.transform.position = leaf.transform.position + new Vector3(0, (leafSpriteRenderer.bounds.size.y / 2), 0);
            leafNode.transform.position = spawnPoint + new Vector3(0, (stemHeight / 2), 0);

            SetLocalScale(leaf, leafScale);

        }
        return newLeaves;
    }

    // Start is called before the first frame update
    void Start()
    {
        stem = transform.parent.gameObject;
        SpriteRenderer stemSprite = stem.GetComponent<SpriteRenderer>();
        spawnPoint = new Vector3(stemSprite.transform.position[0],
                                 stemSprite.transform.position[1],
                                 stemSprite.transform.position[2] + 2) + mulVec(offsetSpawnPoint, transform.root.localScale);

        leaves = CreateLeaves(leafCount);
    }

    public void SetLocalScale(GameObject obj, Vector3 scale)
    {
        obj.transform.parent.localScale = scale;
    }

    public void SetLeavesRotation(float newAngle, float anglePower, float rotationOffset)
    {
        //sets the spread of the leaves evenly around angle.
        //angle power is how much of the total angle is used
        //anglePower is a float from 0 -> 1
        float maxAngle = newAngle * anglePower;
        float interval = maxAngle / (leaves.Length - 1);

        if (debug)
        {
            print("Current max angle: " + maxAngle.ToString());
            print("Angle interval: " + interval.ToString());
        }
        int midInd = leaves.Length / 2;
        if (leaves.Length % 2 == 0)
        {
            midInd--;
        }

        for (int i = 0; i <= midInd; i++)
        {
            //get angles on one side and mirror to the other side
            float zAngle = (maxAngle / 2) - (interval * i);
            zAngle = zAngle - ((zAngle * rotationOffset) * ((i + 1) / (1 + (float)midInd)));
            zAngle = NumOp.Cutoff(zAngle, 0, 360f);

            if (zAngle >= 0)
            {
                Vector3 rotation = new Vector3(0, 0, zAngle);

                leaves[i].transform.parent.rotation = Quaternion.Euler(rotation);
                leaves[leaves.Length - i - 1].transform.parent.rotation = Quaternion.Euler(-rotation);
            }
        }
    }

    public void SetHeightSkew(float skewAmount, float offsetAmount, bool reverseSkew, Vector3 leafScale)
    {
        //sets how much the leaves' height are skewed from the centre
        //offsetAmount represents the amount of skew used the further
        //the leaf is from the center
        //skewAmount and offSetAMount are floats from  0 -> 1

        int midInd = leaves.Length / 2;
        if (leaves.Length % 2 == 0)
        {
            midInd--;
        }

        for (int i = 0; i <= midInd; i++)
        {

            Vector3 interval = leafScale - ((leafScale * skewAmount) * ((i) / ((float)midInd + 1)));

            if (reverseSkew)
            {
                SetLocalScale(leaves[i], interval);
                SetLocalScale(leaves[leaves.Length - i - 1], interval);
            }

            else
            {

                SetLocalScale(leaves[midInd - i], interval);
                SetLocalScale(leaves[leaves.Length - midInd + i - 1], interval);
            }

            if (offsetAmount > 0)
                skewAmount = NumOp.Cutoff(skewAmount + (skewAmount * offsetAmount), 0f, 1f);
        }
    }

    public void SetColor()
    {
        Color chosenColor = GetComponent<TimeToColor>().realTimeColor;
        //color the fore and background components
        for (int i = 0; i < leaves.Length; i++)
        {
            SpriteRenderer sr = leaves[i].GetComponent<SpriteRenderer>();
            Transform tf = leaves[i].transform;
            if (i % 2 != 0)
            {
                tf.parent.position = new Vector3(tf.parent.position[0], tf.parent.position[1], tf.root.position[2] + 12);
                sr.color = chosenColor;
            }
            else
            {
                tf.parent.position = new Vector3(tf.parent.position[0], tf.parent.position[1], tf.root.position[2] + 16);
                sr.color = new Color(chosenColor[0] / 1.5f, chosenColor[1] / 1.5f, chosenColor[2] / 1.5f, chosenColor[3]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //set variables within appropriate ranges
        rotationOffset = NumOp.Cutoff(rotationOffset, 0f, 1f);
        sproutSize = NumOp.Cutoff(sproutSize, 0f, 1f);
        heightOffset = NumOp.Cutoff(heightOffset, 0f, 1f);
        angle = NumOp.Cutoff(angle, 0f, 360f);

        if (leafCount != leaves.Length)
            leaves = CreateLeaves(leafCount);

        float growthAmount = GetComponent<Grow>().growthAmount;

        SetLeavesRotation(angle * growthAmount, sproutSize * growthAmount, rotationOffset * growthAmount);
        SetHeightSkew(heightOffset * growthAmount, heightOffsetPower * growthAmount, invHeightSkew, leafScale * growthAmount);
        SetColor();
    }
}
