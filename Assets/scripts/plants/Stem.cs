using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MiscFunctions;

public class Stem : MonoBehaviour
{
    public Vector2 scaleAmount;

    public bool readGenesOnStart;


    public void ReadGenesOnStart(bool b)
    {
        readGenesOnStart = b;
    }

    // Start is called before the first frame update
    void Start()
    {
        //load genes for fields
        if (readGenesOnStart)
        {
            Genes genes = GetComponent<Genes>();
            if (genes != null)
            {
                try
                {
                    scaleAmount = Parse.Vec2(genes.GetValue<string>("scaleAmount"));
                }
                catch (Exception e)
                {
                    print(e.ToString());
                    Debug.LogWarning("A gene could not be read, some variables may be using default values!", gameObject);
                }
            }
            else
            {
                Debug.LogWarning(String.Format("A gene script was not given to '{0}', using default values!", name), gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float growthAmount = GetComponent<Grow>().growthAmount;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector2 srSizeBefore = new Vector2(sr.bounds.size.x, sr.bounds.size.y);

        transform.localScale = new Vector3(scaleAmount[0]*growthAmount, scaleAmount[1]*growthAmount, transform.localScale[2]);

        Vector2 posIncrease = new Vector2( (sr.bounds.size.x - srSizeBefore[0])/2f, (sr.bounds.size.y - srSizeBefore[1])/2f );
        
        transform.position = transform.position + new Vector3(0, posIncrease[1], 0);
    }
}
