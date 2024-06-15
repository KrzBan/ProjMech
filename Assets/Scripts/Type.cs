using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Type
{
    Sport,
    Chill,
    Zwierzeta,
    Wrazliwosc,
    TOTAL
}

[Serializable]
public struct TypeValue
{
    public Type Type;
    public int Value;
}

