using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetTextAcordingLanguage : MonoBehaviour
{
    [Header("Text Resource settings")]
    public int numText;
    public TextStrings textStrings;
    // Start is called before the first frame update
    void Start()
    {
       SetText();
        MainMenuBehaviour.changeLanguage += OnLanguageChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        MainMenuBehaviour.changeLanguage -= OnLanguageChanged;
    }

    public void OnLanguageChanged() {
       SetText();
        
    }

    public void SetText()
    {
        if (GetComponent<Text>()!=null)
        {
         
            GetComponent<Text>().text = GameManager.getStrings(textStrings)[numText];

        }
        if (GetComponent<TMP_Text>()!=null)
        {
         
            GetComponent<TMP_Text>().text = GameManager.getStrings(textStrings)[numText];

        }
    }
}
