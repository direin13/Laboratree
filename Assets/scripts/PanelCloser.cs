using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    public void ClosePanel()
    {
         this.transform.parent.gameObject.SetActive(false);     //sets panel as inactive to make sure it is not visible to user
    }
}
