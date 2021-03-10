using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FolderHead : MonoBehaviour
{
    public Button[] folderButtons;
    public Color color;
    public Button activeButton;

    // Start is called before the first frame update
    void Start()
    {
        //get all buttons in display
        folderButtons = GetComponentsInChildren<Button>();      
        if (activeButton == null)
            activeButton = folderButtons[0];
    }

    public void HighlightButton(Button button)
    {

        //highlight colour of clicked button
        foreach (Button folderButton in folderButtons)
        {
            Image b_img = folderButton.GetComponent<Image>();
            GameObject drop = folderButton.transform.GetChild(0).gameObject;
            RectTransform fButRect = folderButton.GetComponent<RectTransform>();
            float fButHeight = fButRect.rect.height;

            //transition colour & position when clicked/not clicked
            if (folderButton != button)
            {
                Color newColor = new Color(color[0] - .25f,
                                           color[1] - .25f,
                                           color[2] - .25f,
                                           color[3]);
                b_img.color = newColor;
                drop.transform.position = new Vector3(drop.transform.position[0],
                                                      folderButton.transform.position[1] - (fButHeight / 50),
                                                      drop.transform.position[2]);
                drop.GetComponent<Image>().color = newColor;
            }

            else
            {
                b_img.color = color;
                drop.transform.position = new Vector3(drop.transform.position[0],
                                                      folderButton.transform.position[1] - (fButHeight / 18),
                                                      drop.transform.position[2]);
                drop.GetComponent<Image>().color = color;
            }

        }
    }

    public MainMenu GetMainMenu()
    {
        return transform.parent.gameObject.GetComponent<MainMenu>();
    }


    // Update is called once per frame
    void Update()
    {
        HighlightButton(activeButton);
    }
}
