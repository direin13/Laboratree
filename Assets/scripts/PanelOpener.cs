using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    public GameObject Panel;
    
    public void OpenPanel()
    {

        // Camera camera = Camera.main;
        var parent = GameObject.Find("PlantCollectionPage").transform;

        // var Panel = (GameObject)Instantiate(prefab,parent);
        // // Panel.transform.parent = parent;
        // Panel.transform.SetAsLastSibling();
        
        Panel.transform.SetParent(parent);
        // Panel.transform.position = camera.ScreenToWorldPoint(new Vector2(Screen.width/2,Screen.height/2));
        
        Panel.transform.position = new Vector2(245,145);       
        
        Panel.SetActive(true);
    }

    // public void ClosePanel()
    // {
    //     Panel.SetActive(false);
    // }

}
