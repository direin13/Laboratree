using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    // public GameObject Panel;
    public GameObject Panel;

    void update()
    {

    }

    public void OpenPanel()
    {
        // if(Panel != null)
        // {

        var parent = GameObject.Find("PlantCollectionPage").transform;
        // var Panel = (GameObject)Instantiate(prefab,parent);
        // // Panel.transform.parent = parent;
        // Panel.transform.SetAsLastSibling();
        
        Panel.transform.SetParent(parent);
        Panel.transform.position = new Vector2(350,153);
        Panel.SetActive(true);
        // }
    }

    // public void ClosePanel()
    // {
    //     Panel.SetActive(false);
    // }

}
