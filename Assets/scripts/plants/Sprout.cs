using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class Sprout : MonoBehaviour
{
    //used to sprout sprites from a point across a given angle
    public Sprite sprite;
    public int leafCount;
    public bool enableBackgroundLeaves;
    public bool flipBackground;

    public float angle;
    public float rotationOffset;
    public float sproutSize;
    public bool invHeightSkew;
    public float heightOffset;
    public float heightOffsetPower;

    private Vector3 spawnPoint;
    public int zLayer;
    public readonly int maxZLayer = 30;
    public Vector2 offsetSpawnPoint;
    public Vector2 leafScale;
    public GameObject[] leaves;

    public bool readGenesOnStart;

    public bool debug;


    public void ReadGenesOnStart(bool b)
    {
        readGenesOnStart = b;
        print(b);
    }


    public GameObject[] CreateLeaves(int amount)
    {
        //delete leaves
        foreach (Transform t in transform)
        {
            GameObject obj = t.gameObject;
            if (obj.name.Contains("<<node>>"))
                Destroy(obj);
        }

        GameObject stem = transform.parent.gameObject;
        SpriteRenderer stemSprite = stem.GetComponent<SpriteRenderer>();

        Vector3 offset = new Vector3(offsetSpawnPoint[0] * transform.root.localScale[0], offsetSpawnPoint[1] * transform.root.localScale[1], 0);

        spawnPoint = new Vector3(stemSprite.transform.position[0],
                                 stemSprite.transform.position[1],
                                 stem.transform.position[2] + (float)zLayer - 14) + offset; //move z forward infront of stem

        GameObject[] newLeaves = new GameObject[amount];

        //creating each leaf
        for (int i = 0; i < amount; i++)
        {
            GameObject leaf = new GameObject("Sprout Object");
            newLeaves[i] = leaf;

            leaf.AddComponent<SpriteRenderer>();
            SpriteRenderer leafSpriteRenderer = leaf.GetComponent<SpriteRenderer>();
            leafSpriteRenderer.sprite = sprite;

            //setting the scale and default position of each leaf
            //each leaf has a node (not visible) at the bottom of the sprite.
            //Use this node to move/rotate leaf around an axis
            GameObject leafNode = new GameObject(name + " <<node>>");
            leafNode.transform.parent = gameObject.transform;
            leaf.transform.parent = leafNode.transform;

            float stemHeight = stemSprite.bounds.size.y;
            leaf.transform.position = leaf.transform.position + new Vector3(0, (leafSpriteRenderer.bounds.size.y / 2), 0);
            leaf.transform.position = new Vector3(leaf.transform.position[0], leaf.transform.position[1], 0);
            leafNode.transform.position = spawnPoint + new Vector3(0, (stemHeight / 2), 0);

            SetLocalScale(leaf, new Vector3(leafScale[0], leafScale[1], 1));
        }
        return newLeaves;
    }

    // Start is called before the first frame update
    void Start()
    {
        print("starting");
        //reading gene script for variable values
        if (readGenesOnStart)
        {
            print("resetting");

            leaves = null;
            Genes genes = GetComponent<Genes>();
            if (genes != null)
            {
                try
                {
                    Sprite tmpSprite = Resources.Load<Sprite>("sprites/" + genes.GetValue<string>("sprite"));
                    if (tmpSprite == null)
                    {
                        throw new Exception(String.Format("There's no sprite 'Resources/sprites/{0}'", genes.GetValue<string>("sprite")));
                    }
                    else
                    {
                        sprite = tmpSprite;
                    }
                    leafCount = genes.GetValue<int>("leafCount");
                    angle = genes.GetValue<float>("angle");
                    rotationOffset = genes.GetValue<float>("rotationOffset");
                    sproutSize = genes.GetValue<float>("sproutSize");
                    invHeightSkew = genes.GetValue<bool>("invHeightSkew");
                    heightOffset = genes.GetValue<float>("heightOffset");
                    heightOffsetPower = genes.GetValue<float>("heightOffsetPower");
                    offsetSpawnPoint = Parse.Vec2(genes.GetValue<string>("offsetSpawnPoint"));
                    leafScale = Parse.Vec2(genes.GetValue<string>("leafScale"));

                }
                catch (Exception e)
                {
                    print(e);
                    Debug.LogWarning("A gene could not be read, some variables may be using default values!", gameObject);
                }
            }
            else
            {
                Debug.LogWarning(String.Format("A gene script was not given to '{0}', using default values!", name), gameObject);
            }
        }
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

    public void SetHeightSkew(float skewAmount, float offsetAmount, bool reverseSkew, Vector2 leafScale)
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
            interval = new Vector3(interval[0], interval[1], 1);

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
            if ( !enableBackgroundLeaves || (i % 2 != 0 && !flipBackground || i % 2 == 0 && flipBackground))
            {
                sr.color = chosenColor;
            }
            else
            {
                sr.color = new Color(chosenColor[0] * 0.7f, chosenColor[1] * 0.7f, chosenColor[2] * 0.7f, chosenColor[3]);
                //move leaf back and lower the colour value
                Transform tf = leaves[i].transform.parent;
                tf.position = new Vector3(tf.position[0], tf.position[1], (transform.position[2] + zLayer - 14) + 20);
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
        zLayer = NumOp.Cutoff(zLayer, 0, maxZLayer);

        if (GetComponent<Grow>().hasStarted)
        {
            if ((leaves == null || leafCount != leaves.Length))
                leaves = CreateLeaves(leafCount);

            float growthAmount = GetComponent<Grow>().growthAmount;

            SetLeavesRotation(angle * growthAmount, sproutSize * growthAmount, rotationOffset * growthAmount);
            SetHeightSkew(heightOffset * growthAmount, heightOffsetPower * growthAmount, invHeightSkew, leafScale * growthAmount);
            SetColor();
        }

    }
}
