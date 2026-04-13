using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_2 : MonoBehaviour
{
    public static GameManager_2 instance;
    private bool ended = false;//用来防止重复计分的 需要加这个变量

    [Header("Counts")]
    public int leftCount = 10;
    public int rightCount = 10;

    [Header("Score")]
    public int leftScore = 0;
    public int rightScore = 0;

    [Header("State")]
    public bool gameEnded = false;

    [Header("UI")]
    public TMP_Text leftTMP;
    public TMP_Text rightTMP;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    // ================== LEFT ==================

    public bool CanThrowLeft()
    {
        return !gameEnded && leftCount > 0;
    }

    public void UseLeft()
    {
        if (leftCount <= 0) return;

        leftCount--;
        UpdateUI();
        CheckEnd();
    }

    public void AddScoreLeft()
    {
        leftScore++;
        UpdateUI();
    }

    // ================== RIGHT ==================

    public bool CanThrowRight()
    {
        return !gameEnded && rightCount > 0;
    }

    public void UseRight()
    {
        if (rightCount <= 0) return;

        rightCount--;
        UpdateUI();
        CheckEnd();
    }

    public void AddScoreRight()
    {
        rightScore++;
        UpdateUI();
    }

    // ================== GAME END ==================

    void CheckEnd()
    {
        if (leftCount == 0 && rightCount == 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("本局结束！");
        Debug.Log($"Left: {leftScore}  Right: {rightScore}");

        if (leftScore > rightScore)
        {
            gameEnded = true;
            if (ended) return;
            ended = true;


            MatchData.player1Score++;

            GoNext();
        }
        else if (rightScore > leftScore)
        {
            if (ended) return;
            ended = true;

            MatchData.player2Score++;

            GoNext();
        }
        else
        {
            Debug.Log("平局！重新开始");

            StartCoroutine(RestartRound());
        }
    }

    IEnumerator RestartRound()
    {
        yield return new WaitForSeconds(1.5f);

        leftCount = 10;
        rightCount = 10;

        leftScore = 0;
        rightScore = 0;

        gameEnded = false;

        Debug.Log("新一局开始");

        UpdateUI();
    }

    // ================== UI ==================

    void UpdateUI()
    {
        if (leftTMP != null)
        {
            leftTMP.text =
                $"LEFT\n" +
                $"剩余: {leftCount}\n" +
                $"命中: {leftScore}";
        }

        if (rightTMP != null)
        {
            rightTMP.text =
                $"RIGHT\n" +
                $"剩余: {rightCount}\n" +
                $"命中: {rightScore}";
        }
    }

    void GoNext()
    {
        SceneManager.LoadScene("ResultBridgeScene"
        );
    }
}