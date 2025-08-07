using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{  
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
