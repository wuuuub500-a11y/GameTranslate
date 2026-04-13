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

    private void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (titleText != null)
            titleText.text = "Select Game";

        if (scoreText != null)
            scoreText.text = $"{GameData.playerAName} {GameData.playerAWins} : {GameData.playerBWins} {GameData.playerBName}";

        if (remainingText != null)
        {
            remainingText.text = "Remaining:\n";

            foreach (string game in GameData.remainingGames)
            {
                remainingText.text += game + "\n";
            }
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
}