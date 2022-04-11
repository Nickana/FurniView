using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddObject : MonoBehaviour
{

    private Button button;
    
    private ProgrammManager ProgrammManagerScript;

    private textScript testText;
    
    private void AddObjectFunction()
    {
       
        ProgrammManagerScript.chooseObject = false;
        ProgrammManagerScript.scrollView.SetActive(true);

    }


    void Start()
    {     
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(AddObjectFunction);
    }

    
}
