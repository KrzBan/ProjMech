using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Conversation
{
    [field: SerializeField] public List<ConversationLine> Lines { get; private set; } = new List<ConversationLine>();
}
