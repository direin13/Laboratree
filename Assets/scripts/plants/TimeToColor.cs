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
    public float growthStages;
    public float healthRatio;
    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        
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
        float splitPoint = 0.15f;

        if (pr.Health() >= healthRatio)
        {
            Color.RGBToHSV(earlyColor, out H2, out S2, out V2);
            colorPerc = NumOp.Cutoff(pr.Health() - pr.GrowthAmount(growthStages), 0f, 1f);
        }
        else
        {
            Color.RGBToHSV(lateColor, out H2, out S2, out V2);
            float tmp = (healthRatio - pr.Health()) * (1f / healthRatio);
            colorPerc = NumOp.Cutoff(1f - (pr.GrowthAmount(growthStages) - tmp), 0f, 1f);
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
        healthRatio = NumOp.Cutoff(healthRatio, 0f, 1f);
        alphaValue = NumOp.Cutoff(alphaValue, 0f, 1f);
        growthStages = NumOp.Cutoff(growthStages, 1, growthStages);

        SetColor();
    }
}
