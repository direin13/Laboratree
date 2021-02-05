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
                ColorUtility.TryParseHtmlString(genes.GetValue("earlyColor"), out earlyColor);
                ColorUtility.TryParseHtmlString(genes.GetValue("optimumColor"), out optimumColor);
                ColorUtility.TryParseHtmlString(genes.GetValue("lateColor"), out lateColor);
                healthToColorRatio = float.Parse(genes.GetValue("healthToColorRatio"));
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

    public void SetColor()
    {
        PlantRates pr = transform.root.gameObject.GetComponent<PlantRates>();

        //get percentage of opposite color used based on health. Get dist from this and optimum color 
        //and add that onto the optimum to get a blend of the 2 colors
        float colorPerc;
        float H1, S1, V1;
        Color.RGBToHSV(optimumColor, out H1, out S1, out V1);

        float H2, S2, V2;

        if (pr.Health() >= healthToColorRatio)
        {
            Color.RGBToHSV(earlyColor, out H2, out S2, out V2);
            colorPerc = NumOp.Cutoff(pr.Health() - GetComponent<Grow>().growthAmount, 0f, 1f);
        }
        else
        {
            Color.RGBToHSV(lateColor, out H2, out S2, out V2);
            float tmp = (healthToColorRatio - pr.Health()) * (1f / healthToColorRatio);
            colorPerc = NumOp.Cutoff(1f - (GetComponent<Grow>().growthAmount - tmp), 0f, 1f);
        }

        Vector3 dist = (new Vector3(H2 - H1, S2 - S1, V2 - V1)) * colorPerc;

        realTimeColor = Color.HSVToRGB(H1 + dist[0], S1 + dist[1], V1 + dist[2]);
        realTimeColor = new Color(realTimeColor[0], realTimeColor[1], realTimeColor[2], alphaValue);
        if (debug)
        {
            print(String.Format("Hue1: {0}, Sat1: {1}, Val1: {2}", H1, S1, V1));
            print(String.Format("Hue2: {0}, Sat2: {1}, Val2: {2}", H2, S2, V2));
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