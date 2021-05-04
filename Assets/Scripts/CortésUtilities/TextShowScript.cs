using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextShowScript : MonoBehaviour
{
    [Header("Text Resource settings")]
    public int numText;
    public TextStrings textStrings;
    [Space(10)]
    [Header("Toggle for the gui on off")]
    public bool GuiOn;

    public string extraText="";
    public int textSize;
    [Space(10)]
    [Header("The text to Display on Trigger")]
    [Tooltip("To edit the look of the text Go to Assets > Create > GUIskin. Add the new Guiskin to the Custom Skin proptery. If you select the GUIskin in your project tab you can now adjust the Label section to change this text")]
    public string Text = "Turn Back";

    [Tooltip("This is the window Box's size. It will be mid screen. Add or reduce the X and Y to move the box in Pixels. ")]
    public Rect BoxSize = new Rect( 1, 1, 1920, 1080);

     [Space(10)]
    [Tooltip("To edit the look of the text Go to Assets > Create > GUIskin. Add the new Guiskin to the Custom Skin proptery. If you select the GUIskin in your project tab you can now adjust the font, colour, size etc of the text")]
    public GUISkin customSkin;

    private void Start()
    {
        textSize = 64;
        BoxSize = new Rect( 1, 1, 1280, 720);
    }

    // if this script is on an object with a collider display the GUI
    private void OnTriggerEnter(Collider other)
    {
      
        if (other.tag=="Player")
        {
         
            GuiOn = true;
            StartCoroutine(nameof(TurnOffGuIAfter));
        }
    }

    private IEnumerator TurnOffGuIAfter()
    {
        yield return new WaitForSeconds(2);
        GuiOn = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag=="Player")
        {
         
            GuiOn = false;   
        }
    }
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

 


    void OnGUI()
    {

        if (customSkin != null)
        {
            GUI.skin = customSkin;
        }

        if (GuiOn == true)
        {
           
                guiStyle.fontSize = textSize;
                guiStyle.normal.textColor = Color.white;
                // Make a group on the center of the screen
                GUI.BeginGroup (new Rect ((Screen.width - BoxSize.width) / 2, (Screen.height - BoxSize.height) / 2, BoxSize.width, BoxSize.height));
                // All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

                //  GUI.Label(BoxSize, Text);
                GUI.Label(BoxSize, GameManager.getStrings(textStrings)[numText] + extraText,guiStyle);
             
                // End the group we started above. This is very important to remember!
                GUI.EndGroup ();   
            
             


        }


    }
   
}
