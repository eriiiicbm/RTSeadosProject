using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WikiSelectionMenu : MonoBehaviour
{
    public Text title, description, prices, type, tecnicAspects1, tecnicAspects2;
    public Image image;
    public GameObject buildList, unitsList;
    public List<RTSEntity> buils, units;
    public GameObject buttonEntity;
     public TextStrings factionTitles;
    public int factionTitlePosition = 0;
    public TextStrings factionDescriptions;
    public int factionDescriptionPosition = 0;
    public Sprite factionLogo;

    public void ChangeFaction()
    {
        SetListEntitys(buils, buildList);
        SetListEntitys(units, unitsList);
Debug.Log("aaaa GameManager.getStrings(factionTitles)[factionTitlePosition]" +  GameManager.getStrings(factionTitles)[factionTitlePosition]);
        title.text = GameManager.getStrings(factionTitles)[factionTitlePosition];
        description.text = GameManager.getStrings(factionDescriptions)[factionDescriptionPosition];
        
        image.sprite = factionLogo;

        prices.text = "";
        type.text = "";
        tecnicAspects1.text = "";
        tecnicAspects2.text = "";
    }

    void SetListEntitys(List<RTSEntity> entities, GameObject list)
    {
        for (int i = 0; i< list.transform.childCount; i++)
        {
            Destroy(list.transform.GetChild(i).gameObject);
        }

        foreach (RTSEntity entity in entities)
        {
            GameObject button = Instantiate(buttonEntity, list.transform);

            button.GetComponent<Image>().sprite = entity.Preview;

            if(button.TryGetComponent<WikiMenu>(out WikiMenu wikiMenu))
            wikiMenu.info = entity;

            wikiMenu.titel = title;
            wikiMenu.desciption = description;
            wikiMenu.image = image;
            wikiMenu.Prices = prices;
            wikiMenu.type = type;
            wikiMenu.tecnicAspects1 = tecnicAspects1;
            wikiMenu.tecnicAspects2 = tecnicAspects2;
        }
    }
}
