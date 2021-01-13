using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FolderHeadClick : MonoBehaviour
{
    public void FolderHeadButtonClick()
    {
        MainMenu m_menu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        FolderHead menuFolderButtons = m_menu.folderHead.GetComponent<FolderHead>();
        Button button_clicked = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (m_menu.isOpen != true)
        {
            m_menu.SetOpen(true);
        }
        else if (menuFolderButtons.activeButton == button_clicked)
        {
            m_menu.SetOpen(false);
        }

        menuFolderButtons.activeButton = button_clicked;
    }
}
