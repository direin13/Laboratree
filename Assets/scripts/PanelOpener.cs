using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    public GameObject Panel;
    
    public void OpenPanel()
    {

        //panel needs to be detached from current parent to appear above the scrollview panel
        //parent is new parent for panel
        var parent = GameObject.Find("PlantCollectionPage").transform;

        // var Panel = (GameObject)Instantiate(prefab,parent);
        // // Panel.transform.parent = parent;
        // Panel.transform.SetAsLastSibling();      //set as last in the parent hierarchy to ensure it appears 
        
        Panel.transform.SetParent(parent);  //set to desired parent
        
        //centres panel on screens
        Panel.transform.position = new Vector2(245,145);       
        
        Panel.SetActive(true);      //sets panel as active to make it visible to user
    }

}
