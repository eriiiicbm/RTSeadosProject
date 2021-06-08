using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class ActionsMenu : MonoBehaviour
{
    public static ActionsMenu _instance;
    private bool menuActive = false;
    public GameObject interactionMenu;

    private UnitSelectionHandlerv2 unitSelectionHandlerv2;
    //0  delete, 1 open close
    // Start is called before the first frame update
    void Start()
    {
        if (_instance==null)
        {
            unitSelectionHandlerv2 = FindObjectOfType<UnitSelectionHandlerv2>();
            _instance = this;   
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnDestroyMenu(bool state)
    {
        interactionMenu.transform.GetChild(0).gameObject.SetActive(state);
    }

    public void TurnMenu()
    {
        if (menuActive==false)
        {
            TurnMenuOn();
            return;
            
        }
        TurnMenuOff();
    }

    public void TurnMenuOn()
    {
        TurnDestroyMenu(unitSelectionHandlerv2.SelectedUnits.Count!=0);
        menuActive = true;
    }
    
    public void TurnMenuOff()
    {
        TurnDestroyMenu(false);
        menuActive = false;
    }
}
