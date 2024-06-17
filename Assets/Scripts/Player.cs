using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    
    [Header("Inspector debug view only (DO NOT CHANGE)")]
    [field: SerializeField] public List<Conversation> Conversations { get; set; }
    [field: SerializeField] public bool Selected { get; set; } = false;


    public void Init(int girlsCount)
    {
        Conversations = new List<Conversation>();
        for (int i = 0; i < girlsCount; i++)
        {
            Conversations.Add(new Conversation());
        }
    }
}
