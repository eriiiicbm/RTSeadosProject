using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WikiSelectionMenu : MonoBehaviour
{
    public Text titel, descripcio, prices, type, tecnicAspects1, tecnicAspects2;
    public Image image;
    public GameObject buildList, unitsList;
    public List<RTSEntity> buils, units;
    public GameObject buttonEntity;
    public string titelS, descriptionS;
    public Sprite factionLogo;

    public void ChangeFaction()
    {
        setListEntitys(buils, buildList);
        setListEntitys(units, unitsList);

        titel.text = titelS;
        descripcio.text = descriptionS;
        image.sprite = factionLogo;

        prices.text = "";
        type.text = "";
        tecnicAspects1.text = "";
        tecnicAspects2.text = "";
    }

    void setListEntitys(List<RTSEntity> entities, GameObject list)
    {
        for (int i = 0; i< list.transform.childCount; i++)
        {
            Destroy(list.transform.GetChild(i).gameObject);
        }

        foreach (RTSEntity entity in entities)
        {
            GameObject button = Instantiate(buttonEntity, list.transform);

            button.GetComponent<Image>().sprite = entity.Preview;

            if(button.TryGetComponent<wikiMenu>(out wikiMenu wikiMenu))
            wikiMenu.info = entity;

            wikiMenu.titel = titel;
            wikiMenu.desciption = descripcio;
            wikiMenu.image = image;
            wikiMenu.Prices = prices;
            wikiMenu.type = type;
            wikiMenu.tecnicAspects1 = tecnicAspects1;
            wikiMenu.tecnicAspects2 = tecnicAspects2;
        }
    }
}
