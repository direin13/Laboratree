using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeFollow : MonoBehaviour
{

    private bool FakeParentStatus;
    
    // private Vector3 posOffset;
    // private Quaternion rotOffset;

    private void Start()
    {
        FakeParentStatus = GameObject.Find("PlantCollectionPage").activeSelf;
    }
 
    private void Update()
    {
        if (FakeParentStatus == false){
            gameObject.SetActive(false);
        }

        // var targetPos = FakeParent.position - posOffset;
        // var targetRot = FakeParent.localRotation * rotOffset;

        // transform.position = RotateAroundPivot(targetPos, FakeParent.position, targetRot);
        // transform.localRotation = targetRot;
    }

    // public void SetFakeParent(Transform parent)
    // {
    //     //Offset vector & rotation
    //     posOffset = parent.position - transform.position;
    //     rotOffset = Quaternion.Inverse(parent.localRotation * transform.localRotation);
    //     //Our fake parent
    //     FakeParent = parent;
    // }
 
    // public Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    // {
    //     //Get a direction from the pivot to the point
    //     Vector3 dir = point - pivot;
        
    //     //Rotate vector around pivot
    //     dir = rotation * dir; 
        
    //     //Calc the rotated vector
    //     point = dir + pivot; 
        
    //     return point; 
    // }
}