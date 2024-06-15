using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] private ConversationWindow conversationWindow;
    [SerializeField] private TMP_InputField cardIDInput;
    [SerializeField] private Button cardSendButton;
    [SerializeField] private TMP_Text datesCounter;

    [Header("Settings")] [SerializeField] private List<Sprite> girlAvatars;
    [SerializeField] private List<Player> players;
    [SerializeField] private List<Card> cards;
    [SerializeField] private int rounds = 5;

    [Header("In Game Data (DO NOT CHANGE)")] [SerializeField]
    private Girl girl;

    [SerializeField] private int currentPlayerIndex = 0;
    [SerializeField] private bool continuation = false;
    [SerializeField] private int previousID = 0;
    [SerializeField] private int roundCounter = 0;

    private void Start()
    {
        RandomizeGirl();
        UpdateVisuals();
    }

    public void RandomizeGirl()
    {
        girl = new Girl
        {
            Avatar = girlAvatars[UnityEngine.Random.Range(0, girlAvatars.Count)],
            Description = Settings.Descriptions[UnityEngine.Random.Range(0, Settings.Descriptions.Count)],
        };
        girl.Randomize();
    }

    public void UpdateVisuals()
    {
        girlAvatar.sprite = girl.Avatar;
        girlName.text = $"{girl.Name} {girl.Age}";
        girlDistance.text = $"{girl.Distance} km od ciebie";
        girlAbout.text = girl.Description;

        playerAvatar.sprite = players[currentPlayerIndex].Image;
        datesCounter.text = players[currentPlayerIndex].Dates.ToString();
        conversationWindow.FillConversation(players[currentPlayerIndex].Conversation);
    }

    IEnumerator IUpdateVisuals(float time = 3f)
    {
        yield return new WaitForSecondsRealtime(time);
        UpdateVisuals();
    }

    public void OnCardSendButtonClicked()
    {
        if (!continuation)
        {
            var cardID = int.Parse(cardIDInput.text);
            previousID = cardID;
            var card = cards.FirstOrDefault(c => c.ID == cardID);
            var player = players[currentPlayerIndex];
            var line = new ConversationLine()
            {
                ID = 0,
                Text = card!.Front,
                IsPlayer = true
            };
            player.Conversation.Lines.Add(line);
            conversationWindow.AddLine(line);

            var answer = new ConversationLine()
            {
                ID = 0,
                Text = card!.AnswersFront[Random.Range(0, card!.AnswersFront.Count)],
                IsPlayer = false
            };
            player.Conversation.Lines.Add(answer);
            conversationWindow.AddLine(answer);
        }
        else
        {
            var option = int.Parse(cardIDInput.text);
            var card = cards.FirstOrDefault(c => c.ID == previousID);
            var player = players[currentPlayerIndex];
            var line = new ConversationLine()
            {
                ID = 0,
                Text = card!.Back[option].Text,
                IsPlayer = true
            };
            player.Conversation.Lines.Add(line);
            conversationWindow.AddLine(line);

            var cardOption = card.Back[option];
            
            var score = girl.Traits.FirstOrDefault(t => t.Type == cardOption.TypeValue.Type).Value;
            score = score switch
            {
                0 => -2,
                1 => -1,
                2 => 1,
                3 => 2,
                _ => score
            };
            score *= cardOption.TypeValue.Value;
            player.CurrentScore += score;

            var answerOption = score switch
            {
                < -1 => card!.AnswersBack.FirstOrDefault(c => c.Type == AnswerType.Negative),
                > 1 => card!.AnswersBack.FirstOrDefault(c => c.Type == AnswerType.Positive),
                _ => card!.AnswersBack.FirstOrDefault(c => c.Type == AnswerType.Neutral)
            };

            var answer = new ConversationLine()
            {
                ID = 0,
                Text = answerOption!.Text,
                IsPlayer = false
            };
            player.Conversation.Lines.Add(answer);
            conversationWindow.AddLine(answer);
            
            if (currentPlayerIndex == players.Count - 1)
                roundCounter++;
        }

        if (roundCounter == rounds)
            EndRound();

        cardIDInput.text = "";
        if (!continuation)
            continuation = true;
        else
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
                currentPlayerIndex = 0;
            continuation = false;
            
            StartCoroutine(IUpdateVisuals());
        }
    }

    private void EndRound()
    {
        roundCounter = 0;
        currentPlayerIndex = 0;
        continuation = false;
        RandomizeGirl();

        var index = 0;
        for (var i = 0; i < players.Count; i++)
        {
            if (players[i].CurrentScore > players[index].CurrentScore)
                index = i;
        }
        players[index].Dates++;
        
        foreach (var player in players)
        {
            player.Conversation = new Conversation();
            player.Conversation.Init();
            player.CurrentScore = 0;
        }

        StartCoroutine(IUpdateVisuals());
    }
}
