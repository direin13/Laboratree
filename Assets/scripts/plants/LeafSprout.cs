using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeafSprout : MonoBehaviour
{
    public GameObject stem;
    public Sprite sprite;
    public int leafCount;
    private Vector3 spawnPoint;
    public float angle;
    public float sproutSize;
    public float rotationOffset;
    public float heightOffset;
    public float heightOffsetPower;
    public Vector3 offsetSpawnPoint;
    public Vector3 leafScale;
    public Color color;
    private GameObject [] leaves;

    private float CutoffFloat(float val, float start, float end)
    {
        if (val > end)
            return end;
        else if (val < start)
            return start;
        else
            return val;
    }

    public Vector3 mulVec(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1[0]*vec2[0], vec1[1] * vec2[1], vec1[2] * vec2[2]);
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer stemSprite = stem.GetComponent<SpriteRenderer>();
        spawnPoint = new Vector3(stemSprite.transform.position[0], 
                                 stemSprite.transform.position[1], 
                                 stemSprite.transform.position[2] + 2) + mulVec(offsetSpawnPoint, transform.root.localScale);

        leaves = new GameObject[leafCount];

        //creating each leaf
        for (int i = 0; i < leafCount; i++)
        {
            GameObject leaf = new GameObject("leaf " + i.ToString());
            leaves[i] = leaf;

            leaf.AddComponent<SpriteRenderer>();
            SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;

            //setting the scale and default position of each leaf
            //each leaf has a node (not visible) at the bottom of the sprite.
            //Use this node to move/rotate leaf around an axis
            GameObject leafNode = new GameObject(leaf.name + " node");
            leafNode.transform.parent = gameObject.transform;
            leaf.transform.parent = leafNode.transform;

            float stemHeight = stemSprite.bounds.size.y;
            leaf.transform.position = leaf.transform.position + new Vector3(0, (sr.bounds.size.y/2), 0);
            leafNode.transform.position = spawnPoint + new Vector3(0, (stemHeight / 2), 0);

            SetLocalScale(leaf, leafScale);

        }
        rotationOffset = CutoffFloat(rotationOffset, 0f, 1f);
        sproutSize = CutoffFloat(sproutSize, 0f, 1f);
        heightOffset = CutoffFloat(heightOffset, 0f, 1f);
        angle = CutoffFloat(angle, 0f, 360f);

    }

    public void SetLocalScale(GameObject obj, Vector3 scale)
    {
        obj.transform.parent.localScale = scale;        
    }

    public void SetLeavesRotation(float anglePower)
    {
        //sets the spread of the leaves evenly around angle.
        //angle power is how much of the total angle is used
        //anglePower is a float from 0 -> 1
        float maxAngle = angle * anglePower;
        float interval = maxAngle / (leafCount - 1);
        float midInd = leafCount / 2;

        for (int i = 0; i <= midInd; i++)
        {
            //get angles on one side and mirror to the other side
            float zAngle = (maxAngle / 2) - (interval * i);
            zAngle = zAngle - ( (zAngle * rotationOffset) * ((i+1)/(1+midInd)) );
            Vector3 rotation = new Vector3(0, 0, zAngle);

            leaves[i].transform.parent.rotation = Quaternion.Euler(rotation);
            leaves[leafCount-i-1].transform.parent.rotation = Quaternion.Euler(-rotation);
        }
    }

    public void SetHeightSkew(float skewAmount, float offsetAmount)
    {
        //sets how much the leaves' height are skewed from the centre
        //angle power is how much of the total angle is used
        //skewAmount and offSetAMount are floats from  0 -> 1
        int midInd = leafCount / 2;

        for (int i = 0; i < midInd; i++)
        {
            //loop goes from midIndex to opposite ends so leaves are affected
            //in that order and mirrored on opposite side
            Vector3 interval = leafScale - ((leafScale * skewAmount) *  ( (i+1)/( (float)midInd+1) ));
            SetLocalScale(leaves[midInd-i-1], interval);

            if (leafCount % 2 == 0)
                SetLocalScale(leaves[midInd + i], interval);
            else
                SetLocalScale(leaves[midInd + i + 1], interval);
            if (offsetAmount > 0)
                skewAmount = CutoffFloat(skewAmount + (skewAmount*offsetAmount), 0f, 1f);
        }
    }

    public void SetBackgroundColor()
    {
        for (int i = 0; i < leafCount; i++)
        {
            SpriteRenderer sr = leaves[i].GetComponent<SpriteRenderer>();
            Transform tf = leaves[i].transform;
            if (i % 2 != 0)
            {
                tf.parent.position = new Vector3(tf.parent.position[0], tf.parent.position[1], tf.root.position[2]+12);
                sr.color = color;
            }
            else
            {
                tf.parent.position = new Vector3(tf.parent.position[0], tf.parent.position[1], tf.root.position[2] + 16);
                sr.color = new Color(color[0] / 1.5f, color[1] / 1.5f, color[2] / 1.5f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetLeavesRotation(sproutSize);
        SetHeightSkew(heightOffset, heightOffsetPower);
        SetBackgroundColor();
    }
}
