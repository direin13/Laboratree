using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopUp : MonoBehaviour
{
    public GameObject messageBox;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void PopUpMessage(string message)
    {
        TMP_Text tmpro = messageBox.GetComponent<TMP_Text>();
        tmpro.text = message;
        gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        TMP_Text tmpro = messageBox.GetComponent<TMP_Text>();
        tmpro.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
