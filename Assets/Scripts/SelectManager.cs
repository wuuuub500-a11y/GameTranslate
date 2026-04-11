using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectManager : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text remainingText;
    public TMP_Text scoreText;
    public TMP_Text finalResultText;

    private void Start()
    {
        UpdateUI();

        if (GameData.remainingGames.Count == 0)
        {
            ShowFinalResult();
        }
    }

    void UpdateUI()
    {
        titleText.text = "Select Game";
        scoreText.text = $"{GameData.playerAName} {GameData.playerAWins} : {GameData.playerBWins} {GameData.playerBName}";

        remainingText.text = "Remaining:\n";
        foreach (string game in GameData.remainingGames)
        {
            remainingText.text += game + "\n";
        }
    }

    public void SelectGame(string gameSceneName)
    {
        if (!GameData.remainingGames.Contains(gameSceneName))
            return;

        GameData.selectedGameScene = gameSceneName;
        GameData.remainingGames.Remove(gameSceneName);
        GameData.playedGames.Add(gameSceneName);

        SceneManager.LoadScene(gameSceneName);
    }

    void ShowFinalResult()
    {
        finalResultText.gameObject.SetActive(true);

        if (GameData.playerAWins > GameData.playerBWins)
        {
            finalResultText.text = GameData.playerAName + " Wins!";
        }
        else if (GameData.playerBWins > GameData.playerAWins)
        {
            finalResultText.text = GameData.playerBName + " Wins!";
        }
        else
        {
            finalResultText.text = "Draw!";
        }
    }
}