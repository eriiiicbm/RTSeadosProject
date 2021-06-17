using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsMenu : MonoBehaviour
{
    public static ActionsMenu _instance;
    private bool menuActive = false;
    public GameObject interactionMenu;

    private UnitSelectionHandlerv2 unitSelectionHandlerv2;
    //0  delete, 1, selectall 2 open close
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
        if (!menuActive)
        {
            return;
        } 
        TurnMenuOn();
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
        interactionMenu.transform.GetChild(1).gameObject.SetActive(true);

        menuActive = true;
    }
    
    public void TurnMenuOff()
    {
        TurnDestroyMenu(false);
        interactionMenu.transform.GetChild(1).gameObject.SetActive(false);

        menuActive = false;
    }
}
