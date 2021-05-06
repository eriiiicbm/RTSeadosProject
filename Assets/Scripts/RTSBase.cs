using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBase : MonoBehaviour
{

    int health;
    string entityName;
    int maxHealth;
    string description;
    Sprite preview;
    GameObject prefab;
    public RTSEntity rtsEntity;

    public void Start()
    {
        
    }

    void SetColor() { }
    void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);

    }
    void TakeDamage(UnitController enemy, float damage)
    {
        StartCoroutine(Flasher(GetComponent<Renderer>().material.color));

    }
    IEnumerator Flasher(Color defaultColor)
    {
        var renderer = GetComponent<Renderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.gray;
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            yield return new WaitForSeconds(.05f);
        }
    }
}
