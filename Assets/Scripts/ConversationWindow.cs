using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationWindow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject conversationLinePrefab;
    [SerializeField] private Transform conversationContainer;
    
    public void AddLine(ConversationLine line)
    {
        var lineUI = Instantiate(conversationLinePrefab, conversationContainer).GetComponent<ConversationLineUI>();
        lineUI.SetLine(line);
    }
    
    public void Clear()
    {
        foreach (Transform child in conversationContainer)
            Destroy(child.gameObject);
    }
    
    public void FillConversation(Conversation conversation)
    {
        Clear();
        
        foreach (var line in conversation.Lines)
            AddLine(line);
    }
}
