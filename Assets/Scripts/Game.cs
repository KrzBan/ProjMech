using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Button skipTurnButton;

    [SerializeField] private CanvasGroup playerSelectCanvasGroup;
    [SerializeField] private CanvasGroup gameCanvasGroup;
    [SerializeField] private CanvasGroup endGameCanvasGroup;

    [SerializeField] private Transform playerSelectPanel;
    [SerializeField] private GameObject playerPanelPrefab;

    [SerializeField] private Transform resultsPanel;
    [SerializeField] private GameObject resultsPrefab;

    [SerializeField] private CanvasGroup victoryPanelCanvasGroup;
    [SerializeField] private TMP_Text victoryText;


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
        playerSelectCanvasGroup.interactable = true;
        playerSelectCanvasGroup.blocksRaycasts = true;
        playerSelectCanvasGroup.alpha = 1.0f;

        gameCanvasGroup.interactable = false;
        gameCanvasGroup.blocksRaycasts = false;
        gameCanvasGroup.alpha = 0.0f;

        endGameCanvasGroup.interactable = false;
        endGameCanvasGroup.blocksRaycasts = false;
        endGameCanvasGroup.alpha = 0.0f;

        players.ForEach(player => {
            var panel = Instantiate(playerPanelPrefab, playerSelectPanel);
            panel.GetComponent<PlayerSelectPanel>().SetupUI(player.Image, player.Name, 
                (bool setActive) => { player.Selected = setActive; });

        });
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

    public void OnCardSendButtonClicked()
    {
        // Card select
        int cardID = 0;
        bool error = false;
        try {
            cardID = int.Parse(cardIDInput.text);
        }
        catch {
            error = true;
        }

        if( cardID < 0 || cardID >= cards.Count ) {
            error = true;
        }

        if (error) {
            cardIDInput.text = string.Empty;
            return;
        }

        var card = cards.FirstOrDefault(c => c.ID == cardID);
        StartCoroutine(IOnPlayerInput(card));
    }

    IEnumerator IOnPlayerInput(Card card)
    {
        cardIDInput.interactable = false;
        cardSendButton.interactable = false;
        skipTurnButton.interactable = false;

        var player = players[currentPlayerIndex];
        var line = new ConversationLine()
        {
            ID = 0,
            Text = card!.Line,
            IsPlayer = true
        };
        player.Conversations[currentGirlId].Lines.Add(line);
        conversationWindow.AddLine(line);

        UpdateVisuals();
        yield return new WaitForSecondsRealtime(1.0f);
        StartCoroutine(IOnGirlResponse(card));
    }

    IEnumerator IOnGirlResponse(Card card)
    {
        var score = currentGirl.Traits.FirstOrDefault(t => t.Type == card.Trait).Value;
        score = score switch
        {
            1 => -2,
            2 => -1,
            3 => 1,
            4 => 2,
            _ => throw new Exception("Invalid trait value")
        };
        score *= card.Strength;
        currentGirl.Affections[currentPlayerIndex] += score;

        var answerOption = score switch
        {
            < 0 => Settings.AnswersNegative[Random.Range(0, Settings.AnswersNegative.Count)],
            _ => Settings.AnswersPositive[Random.Range(0, Settings.AnswersPositive.Count)]
        };

        if (string.IsNullOrEmpty(card.CustomAsnwer) == false)
        {
            answerOption = card.CustomAsnwer;
        }

        var answer = new ConversationLine()
        {
            ID = 0,
            Text = answerOption,
            IsPlayer = false
        };
        players[currentPlayerIndex].Conversations[currentGirlId].Lines.Add(answer);
        conversationWindow.AddLine(answer);

        UpdateVisuals();
        yield return new WaitForSecondsRealtime(1.0f);
        EndTurn();
    }

    private void EndTurn()
    {
        ++currentPlayerIndex;
        if (currentPlayerIndex >= players.Count)
        {
            EndRound();
        }

        if (roundCounter >= rounds)
        {
            StartCoroutine(IEndGame());
        }

        UpdateVisuals();

        cardIDInput.interactable = true;
        cardSendButton.interactable = true;
        skipTurnButton.interactable = true;
    }

    private void EndRound()
    {
        roundCounter++;
        currentPlayerIndex = 0;

        RandomizeGirl();
    }
    private IEnumerator IEndGame()
    {
        gameCanvasGroup.interactable = false;
        gameCanvasGroup.blocksRaycasts = false;
        gameCanvasGroup.alpha = 0.0f;

        endGameCanvasGroup.interactable = true;
        endGameCanvasGroup.blocksRaycasts = true;
        endGameCanvasGroup.alpha = 1.0f;

        var score = new List<int>(new int[players.Count]);

        girls.ForEach(girl =>
        {
            var maxAffection = girl.Affections.Max();
            var favoritePlayerIds = new List<int>();
            for(int i = 0; i < girl.Affections.Count; i++)
            {
                if (girl.Affections[i] == maxAffection)
                {
                    favoritePlayerIds.Add(i);
                }
            }

            foreach (int favoritePlayerId in favoritePlayerIds)
            {
                ++score[favoritePlayerId];
            }

            var favoritePlayer = players[favoritePlayerIds[0]];
            var panel = Instantiate(resultsPrefab, resultsPanel);
            panel.GetComponent<EndGirlPanel>().SetupUI(girl.Avatar, girl.Name,
                favoritePlayer.Image, favoritePlayer.Name);
        });

        var maxScore = score.Max();
        var winnerNames = new List<string>();

        for(var i = 0; i < score.Count; ++i)
        {
            if (score[i] == maxScore)
            {
                winnerNames.Add(players[i].Name);
            }
        }

        yield return new WaitForSecondsRealtime(4.0f);

        victoryPanelCanvasGroup.alpha = 1.0f;
        string text = winnerNames.Count == 1 ? "Zwyciężcą został:\n" : "Zwyciężcami są:\n";
        winnerNames.ForEach(name => { text += name; text += '\n'; });
        victoryText.text = text;
    }

    public void StartGameUI()
    {
        var selectedPlayers = players.Where(p => p.Selected);
        if (selectedPlayers.Count() < 2) return;

        players = selectedPlayers.ToList();

        players.ForEach(p => { p.Init(girls.Count); });
        girls.ForEach(g => { g.Init(players.Count); });

        // Shuffle players
        players= players.OrderBy(_ => Random.Range(0, 1000)).ToList();

        RandomizeGirl();
        UpdateVisuals();

        playerSelectCanvasGroup.alpha = 0.0f;
        playerSelectCanvasGroup.blocksRaycasts = false;
        playerSelectCanvasGroup.interactable = false;

        gameCanvasGroup.alpha = 1.0f;
        gameCanvasGroup.blocksRaycasts = true;
        gameCanvasGroup.interactable = true;

    }

    public void SkipTurn()
    {
        EndTurn();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
