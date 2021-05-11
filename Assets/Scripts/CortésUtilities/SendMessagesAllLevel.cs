using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessagesAllLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public   void ChangeLanguage(){
        gameObject.BroadcastMessage("OnLanguageChanged");
    }
}
