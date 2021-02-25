using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowFolderPage : MonoBehaviour
{
    public GameObject page;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = page.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (page != null)
        {
            FolderHead fh = transform.parent.gameObject.GetComponent<FolderHead>();
            if ( (fh.GetMainMenu().isOpen || fh.GetMainMenu().inTransition) && fh.activeButton == GetComponent<Button>())
            {
                page.SetActive(true);
                page.transform.localPosition = startPos;
            }
            else
            {
                page.transform.localPosition = new Vector3(startPos[0] - (float)Screen.width*5, 
                                                 startPos[1] - (float)Screen.height*5, 
                                                 0);
                page.SetActive(false);
            }
        }
    }
}
