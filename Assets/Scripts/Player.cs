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
    [field: SerializeField] public Conversation Conversation { get; set; }
    [field: SerializeField] public int CurrentScore { get; set; }
    [field: SerializeField] public int Dates { get; set; }
}
