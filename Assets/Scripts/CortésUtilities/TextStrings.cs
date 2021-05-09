using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StringAsset", menuName = "new StringAsset")]
public class TextStrings : ScriptableObject
{
    [SerializeField]  public List<string> stringsEn;
    [SerializeField]  public List<string> stringsEs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
