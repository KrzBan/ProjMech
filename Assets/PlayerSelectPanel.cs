using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerSelectPanel : MonoBehaviour
{
    [SerializeField] private Image PlayerImage;
    [SerializeField] private TMP_Text PlayerName;
    [SerializeField] private Toggle PlayerToggle;

    public void Start()
    {
        PlayerToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }
    public void SetupUI(Sprite playerImage, string playerName, UnityAction<bool> callback)
    {
        PlayerImage.sprite = playerImage;
        PlayerName.text = playerName;
        PlayerToggle.onValueChanged.AddListener(callback);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        ColorBlock cb = PlayerToggle.colors;
        if (isOn)
        {
            cb.normalColor = Color.green;
            cb.highlightedColor = Color.green;
            cb.selectedColor = Color.green;
        }
        else
        {
            cb.normalColor = Color.gray;
            cb.highlightedColor = Color.gray;
            cb.selectedColor = Color.gray;
        }
        PlayerToggle.colors = cb;
    }
}
