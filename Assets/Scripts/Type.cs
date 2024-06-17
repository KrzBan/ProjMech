using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TypeUtils
{
    public static string TypeToString(Type type)
    {
        switch (type)
        {
            case Type.Sport: return "Sport";
            case Type.Chill: return "Chill";
            case Type.Zwierzeta: return "Zwierzęta";
            case Type.Wrazliwosc: return "Wrażliwość";
        }
        throw new Exception("Unknown type");
    }
}


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
public class TypeValue
{
    public Type Type;
    public int Value;
    public bool Revealed;
}

