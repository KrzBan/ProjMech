using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image girlAvatar;

    [SerializeField] private TMP_Text girlName;
    [SerializeField] private TMP_Text girlDistance;
    [SerializeField] private TMP_Text girlAbout;

    [SerializeField] private Image playerAvatar;
    [SerializeField] private TMP_Text playerName;

    [SerializeField] private ConversationWindow conversationWindow;
    [SerializeField] private TMP_InputField cardIDInput;
    [SerializeField] private Button cardSendButton;

    [Header("Settings")] 
    [SerializeField] private List<Player> players;
    [SerializeField] private List<Card> cards;
    [SerializeField] private int rounds = 5;

    [SerializeField] private List<Girl> girls;


    [Header("In Game Data (DO NOT CHANGE)")] 
    [SerializeField] private Girl currentGirl;
    [SerializeField] private int currentGirlId = -1;

    [SerializeField] private int currentPlayerIndex = 0;
    [SerializeField] private int roundCounter = 0;

    private void Start()
    {
        players.ForEach(p => { p.Init(girls.Count); });
        girls.ForEach(g => { g.Init(players.Count); });

        var shuffledcards = players.OrderBy(_ => Random.Range(0, 1000)).ToList();

        RandomizeGirl();
        UpdateVisuals();
    }

    public void RandomizeGirl()
    {
        int newGirlId = currentGirlId;
        while(newGirlId == currentGirlId)
        {
            newGirlId = Random.Range(0, girls.Count);
        }
        currentGirlId = newGirlId;
        currentGirl = girls[currentGirlId];
    }

    public void UpdateVisuals()
    {
        girlAvatar.sprite = currentGirl.Avatar;
        girlName.text = $"{currentGirl.Name} {currentGirl.Age}";
        girlDistance.text = $"{currentGirl.Distance} km od ciebie";
        girlAbout.text = currentGirl.Description;

        playerAvatar.sprite = players[currentPlayerIndex].Image;
        playerName.text = players[currentPlayerIndex].Name;
        conversationWindow.FillConversation(players[currentPlayerIndex].Conversations[currentGirlId]);

        cardIDInput.text = "";
    }

    IEnumerator IUpdateVisuals(float time = 3f)
    {
        yield return new WaitForSecondsRealtime(time);
        UpdateVisuals();
    }

    public void OnCardSendButtonClicked()
    {
        // Card select
        var cardID = int.Parse(cardIDInput.text);
        var card = cards.FirstOrDefault(c => c.ID == cardID);
        var player = players[currentPlayerIndex];
        var line = new ConversationLine()
        {
            ID = 0,
            Text = card!.Line,
            IsPlayer = true
        };
        player.Conversations[currentGirlId].Lines.Add(line);
        conversationWindow.AddLine(line);
        
        var score = currentGirl.Traits.FirstOrDefault(t => t.Type == card.Trait).Value;
        score = score switch
        {
            0 => -2,
            1 => -1,
            2 => 1,
            3 => 2,
            _ => score
        };
        score *= card.Strength;
        currentGirl.Affections[currentPlayerIndex] += score;

        var answerOption = score switch
        {
            < -1 => Settings.AnswersNegative[Random.Range(0, Settings.AnswersNegative.Count)],
            > 1 => Settings.AnswersPositive[Random.Range(0, Settings.AnswersPositive.Count)],
            _ => Settings.AnswersNeutral[Random.Range(0, Settings.AnswersNeutral.Count)]
        };

        if(string.IsNullOrEmpty(card.CustomAsnwer) == false)
        {
            answerOption = card.CustomAsnwer;
        }

        var answer2 = new ConversationLine()
        {
            ID = 0,
            Text = answerOption,
            IsPlayer = false
        };
        player.Conversations[currentGirlId].Lines.Add(answer2);
        conversationWindow.AddLine(answer2);

        ++currentPlayerIndex;
        if (currentPlayerIndex >= players.Count)
        {
            EndRound();
        }   
        
        if (roundCounter >= rounds)
        {
            EndGame();
        }
           
        StartCoroutine(IUpdateVisuals(0f));
    }

    private void EndRound()
    {
        roundCounter++;
        currentPlayerIndex = 0;

        RandomizeGirl();
    }
    private void EndGame()
    {
        // Game finished, select winner
    }
}
