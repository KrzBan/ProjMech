using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Engine/Card")]
public class Card : ScriptableObject
{
    public int ID;
    public string Line;

    public Type Trait;
    public int Strength;

    [Header("Optional")]
    public string CustomAsnwer;
}
