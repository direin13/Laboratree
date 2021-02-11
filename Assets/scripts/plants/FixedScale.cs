using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixedScale : MonoBehaviour
{

    public float FixeScale = 1;

    public float getScale(float scale)
    {
        if (scale != 0)
        {
            return FixeScale / scale;
        }
        else
        {
            return 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        float xScale = getScale(transform.parent.localScale.x);
        float yScale = getScale(transform.parent.localScale.y);
        float zScale = getScale(transform.parent.localScale.z);

        transform.localScale = new Vector3(xScale, yScale, zScale);
    }
}