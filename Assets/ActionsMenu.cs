using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionsMenu : MonoBehaviour
{
    public static ActionsMenu _instance;
    private bool menuActive = false;
    public GameObject interactionMenu;

    private UnitSelectionHandlerv2 unitSelectionHandlerv2;
    //0  delete, 1, selectall 2 enableDisable automatic behaviour 3 open close 
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
        interactionMenu.transform.GetChild((int)MenuOptions.Delete).gameObject.SetActive(state);
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
        bool hasSelectedUnits= unitSelectionHandlerv2.SelectedUnits.Count!=0 ;
        TurnDestroyMenu(hasSelectedUnits);
        interactionMenu.transform.GetChild((int)MenuOptions.OpenClose).gameObject.SetActive(true);
        if (!hasSelectedUnits)
        {
            return;
        }
        interactionMenu.transform.GetChild((int) MenuOptions.AutomaticBehaviour).transform.GetChild(0)
            .GetComponent<TMP_Text>().text = $"AutomaticAttackSelected {unitSelectionHandlerv2.CheckIfAllSelectedUnitsHaveTheAutomaticBehaviour()}";
         interactionMenu.transform.GetChild((int)MenuOptions.AutomaticBehaviour).gameObject.SetActive(true);

// todounitSelectionHandlerv2.SelectedUnits.Count 
        menuActive = true;
    }
    
    public void TurnMenuOff()
    {
        TurnDestroyMenu(false);
        interactionMenu.transform.GetChild((int)MenuOptions.OpenClose).gameObject.SetActive(false);

        menuActive = false;
        
        interactionMenu.transform.GetChild((int)MenuOptions.AutomaticBehaviour).gameObject.SetActive(false);
    }


    public void ToggleAutomaticBehaviour()
    {
        TMP_Text  name = interactionMenu.transform.GetChild((int) MenuOptions.AutomaticBehaviour).transform.GetChild(0)
            .GetComponent<TMP_Text>();
    if (name.text.Contains("False"))
    {
        name.text = "AutomaticAttackSelected True";
        unitSelectionHandlerv2.ToggleAutomaticBehaviour(false);
        return;
    } 
    name.text = "AutomaticAttackSelected False";
    unitSelectionHandlerv2.ToggleAutomaticBehaviour(true);

    
    }
}

  enum MenuOptions
{
    Delete=0, SelectAll=1, AutomaticBehaviour=2,OpenClose=3
}
