using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultBridgeManager : MonoBehaviour
{
    public TMP_Text infoText;
    public float waitTime = 1.5f;

    private void Start()
    {
        StartCoroutine(ProcessResult());
    }

    private System.Collections.IEnumerator ProcessResult()
    {
        // 先检查缓存结果
        if (MiniGameResultCache.winner == 1)
        {
            GameData.playerAWins++;

            if (infoText != null)
                infoText.text = GameData.playerAName + " Win!";
        }
        else if (MiniGameResultCache.winner == 2)
        {
            GameData.playerBWins++;

            if (infoText != null)
                infoText.text = GameData.playerBName + " Win!";
        }
        else
        {
            if (infoText != null)
                infoText.text = "No result";
        }

        yield return new WaitForSeconds(waitTime);

        // 用完立刻清空，防止下次串数据
        MiniGameResultCache.Clear();

        // 如果五个游戏都玩完了，就进最终结算
        if (GameData.remainingGames.Count == 0)
        {
            SceneManager.LoadScene("FinalResultScene");
        }
        else
        {
            SceneManager.LoadScene("SelectScene");
        }
    }
}