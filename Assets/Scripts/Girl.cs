using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Girl", menuName = "Engine/Girl")]
public class Girl : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public int Distance { get; private set; }
    [field: SerializeField] public int Age { get; private set; }
    [field: SerializeField] public Sprite Avatar { get; set; }
    [field: SerializeField] public List<Type> Types { get; private set; } 
    [field: SerializeField] public List<TypeValue> Traits { get; private set; }
    [field: SerializeField] public List<int> Affections { get; private set; }


    public void Init(int playerCount)
    {
        Affections = new List<int> ( new int[playerCount] );
        RandomizeTraits();
    }

    public void RandomizeTraits()
    {
        var typeCount = Random.Range(2, 4);
        Types = new List<Type>();
        for (var i = 0; i < typeCount; i++)
        {
            var type = (Type)Random.Range(0, (int)Type.TOTAL);
            
            while (Types.Contains(type))
                type = (Type)Random.Range(0, (int)Type.TOTAL);
            
            Types.Add(type);
        }
        
        Traits = new List<TypeValue>();
        var val = 4;
        while (val > 0)
        {
            var type = (Type)Random.Range(0, (int)Type.TOTAL);
            var contains = false;
            for (var i = 0; i < Traits.Count; i++)
            {
                if (Traits[i].Type == type)
                {
                    contains = true;
                    break;
                }
            }
            if (contains)
                continue;
            
            Traits.Add(new TypeValue{Type = type, Value = val});
            
            val--;
        }
    }
}
