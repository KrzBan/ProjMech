using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Engine/Card")]
public class Card : ScriptableObject
{
    public int ID;
    public string Front;
    public List<string> AnswersFront;
    public List<CardOption> Back;
    public List<Answer> AnswersBack;
}
