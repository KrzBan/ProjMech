using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum AnswerType
{
    Positive,
    Neutral,
    Negative
}

[Serializable]
public class Answer
{
    public AnswerType Type;
    public string Text;
}
