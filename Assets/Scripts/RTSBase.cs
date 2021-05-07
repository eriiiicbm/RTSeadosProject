using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBase : MonoBehaviour
{
   protected UnitStates currentState;
    string entityName;
    int maxHealth;
    string description;
    Sprite preview;
    float health;
    GameObject prefab;
    public RTSEntity rtsEntity;

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    public float Health { get => health; set => health = value; }

    public void Start()
    {
        currentState = UnitStates.Idle;
        health = maxHealth;
    }

    public virtual void SetColor() { }
    void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);

    }
    public void TakeDamage(UnitCombat enemy, float damage)
    {
        health = health - damage;
        StartCoroutine(Flasher(GetComponent<Renderer>().material.color));


    }
    IEnumerator Flasher(Color defaultColor)
    {
        var renderer = GetComponent<Renderer>();
        if (health != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                renderer.material.color = Color.gray;
                yield return new WaitForSeconds(.05f);
                renderer.material.color = defaultColor;
                yield return new WaitForSeconds(.05f);
            }
        }
        else
        {
            Destroy(this.transform.parent.gameObject);

        }
    }
    void GoToNextState()
    {
        string methodName = this.currentState.ToString() + "State";
        Debug.Log("STATE METHOD NAME " + methodName);
        SendMessage(methodName);
    }
    public virtual IEnumerator IdleState() {
        Debug.LogError("OverrideThisMethod before use it");
        yield return new WaitForEndOfFrame();
    }
    

}
