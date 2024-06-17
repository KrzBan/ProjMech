using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EndGirlPanel : MonoBehaviour
{
    [SerializeField] private Image GirlImage;
    [SerializeField] private TMP_Text GirlName;
    [SerializeField] private Image PlayerImage;
    [SerializeField] private TMP_Text PlayerName;

    public void SetupUI(Sprite girlImage, string girlName, Sprite playerImage, string playerName)
    {
        GirlImage.sprite = girlImage;
        GirlName.text = girlName;
        PlayerImage.sprite = playerImage;
        PlayerName.text = playerName;
    }
}
