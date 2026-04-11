using UnityEngine;
using TMPro;

public class FinalResultManager : MonoBehaviour
{
    public TMP_Text finalText;
    public TMP_Text scoreText;

    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{GameData.playerAName}  {GameData.playerAWins} : {GameData.playerBWins}  {GameData.playerBName}";
        }

        if (finalText != null)
        {
            if (GameData.playerAWins > GameData.playerBWins)
            {
                finalText.text = GameData.playerAName + " WIN";
            }
            else if (GameData.playerBWins > GameData.playerAWins)
            {
                finalText.text = GameData.playerBName + " WIN";
            }
            else
            {
                finalText.text = "Æ½¾Ö£¡";
            }
        }
    }
}