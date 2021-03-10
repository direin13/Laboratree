using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FolderHeadClick : MonoBehaviour
{
    public void FolderHeadButtonClick()
    {
        //get components menu components
        MainMenu m_menu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        FolderHead menuFolder = m_menu.folderHead.GetComponent<FolderHead>();
        Button button_clicked = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        //set status of folderhead 
        if (m_menu.isOpen != true)
        {
            m_menu.SetOpen(true);
        }
        else if (menuFolder.activeButton == button_clicked)
        {
            m_menu.SetOpen(false);
        }

        menuFolder.activeButton = button_clicked;       //set button status
    }
}
