using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour
{
    public float scaleFactor;
    public GameObject folderHead;
    private Vector3 followPoint;
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        Transform tr = gameObject.transform;
        scaleFactor = (Screen.height / tr.root.gameObject.GetComponent<CanvasScaler>().referenceResolution.x) * 10;
        ChangePosition(new Vector3(GetPosition()[0], 6f, 0));
        ChangeFollowPoint(GetPosition());
        SetOpen(isOpen);
    }

    public void ChangeFollowPoint(Vector3 new_pos)
    {
        followPoint = new_pos * scaleFactor;
    }

    public Vector3 GetPosition()
    {
        return gameObject.GetComponent<RectTransform>().position;
    }

    public void ChangePosition(Vector3 new_vec)
    {
        gameObject.GetComponent<RectTransform>().position = new_vec;
    }


    public void followY(Vector3 point, float speed)
    {
        Vector3 curr_pos = GetPosition();
        float y_dist = curr_pos[1] - point[1];
        if (y_dist > 0)
        {
            //bove the follow point
            ChangePosition(GetPosition() + new Vector3(0, -speed, 0));
            curr_pos = GetPosition();
            if (curr_pos[1] < point[1])
            {
                ChangePosition(new Vector3(curr_pos[0], point[1], curr_pos[2]));
            }
        }
        else if (y_dist < 0)
        {
            //below the follow point
            ChangePosition(GetPosition() + new Vector3(0, speed, 0));
            curr_pos = GetPosition();
            if (curr_pos[1] > point[1])
            {
                ChangePosition(new Vector3(curr_pos[0], point[1], curr_pos[2]));
            }
        }
        //else level
    }


    public void SetOpen(bool b)
    {
        if (b)
        {
            isOpen = true;
            ChangeFollowPoint(new Vector3(0, 110f, 0));
        }

        else
        {
            isOpen = false;
            ChangeFollowPoint(new Vector3(0, 6f, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        float followSpeed = (float)(Math.Round(300f * Time.deltaTime, 2)) * scaleFactor;
        followY(followPoint, followSpeed);
    }
}
