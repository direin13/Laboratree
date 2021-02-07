using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiscFunctions;
using System;

public class TimeToColor : MonoBehaviour
{
    /// <summary>
    /// Used to set color in realtime time depending on plant's level of growth and health
    /// </summary>
    public Color earlyColor;
    public Color optimumColor;
    public Color lateColor;
    public Color realTimeColor;
    public float alphaValue;
    public float healthToColorRatio; //when color starts to change in the lifecycle of the object
    public bool setSpriteRendererColor;
    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        Genes genes = GetComponent<Genes>();
        if (genes != null)
        {
            try
            {
                ColorUtility.TryParseHtmlString(genes.GetValue<string>("earlyColor"), out earlyColor);
                ColorUtility.TryParseHtmlString(genes.GetValue<string>("optimumColor"), out optimumColor);
                ColorUtility.TryParseHtmlString(genes.GetValue<string>("lateColor"), out lateColor);
                healthToColorRatio = genes.GetValue<float>("healthToColorRatio");
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

    public void SetColor()
    {
        //get percentage of opposite color used based on health. Get that percentage of the dist from this and optimum color 
        //and add that onto the optimum to get a blend of the 2 colors

        PlantRates pr = transform.root.gameObject.GetComponent<PlantRates>();
        float colorPerc;

        if (pr.Health() >= healthToColorRatio)
        {
            colorPerc = NumOp.Cutoff(pr.Health() - GetComponent<Grow>().growthAmount, 0f, 1f);
            realTimeColor = NumOp.GetColorBlend(optimumColor, earlyColor, colorPerc);
        }
        else
        {
            float tmp = (healthToColorRatio - pr.Health()) * (1f / healthToColorRatio);
            colorPerc = NumOp.Cutoff(1f - (GetComponent<Grow>().growthAmount - tmp), 0f, 1f);
            realTimeColor = NumOp.GetColorBlend(optimumColor, lateColor, colorPerc);
        }

        realTimeColor = new Color(realTimeColor[0], realTimeColor[1], realTimeColor[2], alphaValue);
        if (debug)
        {
            print("percentage of opposite used: " + colorPerc.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthToColorRatio = NumOp.Cutoff(healthToColorRatio, 0f, 1f);
        alphaValue = NumOp.Cutoff(alphaValue, 0f, 1f);

        SetColor();
        if (setSpriteRendererColor)
        {
            GetComponent<SpriteRenderer>().color = realTimeColor;
        }
    }
}