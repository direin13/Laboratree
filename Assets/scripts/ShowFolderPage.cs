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
        startPos = page.transform.localPosition;        //set starting position
    }

    // Update is called once per frame
    void Update()
    {
        if (page != null)
        {
            FolderHead fh = transform.parent.gameObject.GetComponent<FolderHead>();     //get folderhead
            if ( (fh.GetMainMenu().isOpen || fh.GetMainMenu().inTransition) && fh.activeButton == GetComponent<Button>())       //if folderhead is active
            {
                page.SetActive(true);       //make designated page visible to user
                page.transform.localPosition = startPos;        //set page's pos to start pos
            }
            else
            {
                page.transform.localPosition = new Vector3(startPos[0] - (float)Screen.width*5, 
                                                 startPos[1] - (float)Screen.height*5,      
                                                 0);        //set position to new position
                page.SetActive(false);      //make page invsible
            }
        }
    }
}
