using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConversationLineUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetLine(ConversationLine line)
    {
        text.text = line.Text;
        if (!line.IsPlayer)
            text.alignment = TextAlignmentOptions.MidlineLeft;
    }
}
