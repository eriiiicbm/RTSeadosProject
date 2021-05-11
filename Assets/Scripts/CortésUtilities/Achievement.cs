using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "New Achievement", menuName = "new Achievement")]
public class Achievement : ScriptableObject
{
    [SerializeField] private bool isObtained;
    [SerializeField] private int messagePos;
    [SerializeField] private TextStrings  messageText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsObtained
    {
        get => isObtained;
        set => isObtained = value;
    }

    public int MessagePos
    {
        get => messagePos;
        set => messagePos = value;
    }

    public TextStrings MessageText
    {
        get => messageText;
        set => messageText = value;
    }
}
