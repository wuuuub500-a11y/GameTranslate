using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_1 : MonoBehaviour
{
    public static GameManager_1 Instance;

    private bool ended = false;//用来防止重复计分的 需要加这个变量

    [Header("时间设置")]
    public float gameTime = 60f;
    private float timer;
    private bool isGameOver;

    [Header("胜负条件")]
    public int dirtyThreshold = 5;

    private List<Tile> allTiles = new List<Tile>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = gameTime;

        // 自动找到所有Tile
        allTiles.AddRange(FindObjectsOfType<Tile>());
    }

    void Update()
    {
        if (isGameOver) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            EndGame();
        }
    }

    void EndGame()
    {
        isGameOver = true;

        int dirtyCount = 0;

        foreach (Tile t in allTiles)
        {
            if (t.currentObject != null)
                dirtyCount++;
        }

        Debug.Log("不干净格子数量: " + dirtyCount);

        if (dirtyCount >= dirtyThreshold)
        {
            if (ended) return;
            ended = true;


            MatchData.player2Score++;

            GoNext();
        }
        else
        {
            if (ended) return;
            ended = true;

            MatchData.player1Score++;

            GoNext();
        }
    }

    // 给UI用
    public float GetTime()
    {
        return timer;
    }

    void GoNext()
    {
        MatchData.currentGameIndex++;

        if (MatchData.currentGameIndex >= MatchData.gameScenes.Length)
        {
            // 总结算
            if (MatchData.player1Score > MatchData.player2Score)
                SceneManager.LoadScene(MatchData.p1WinScene);
            else
                SceneManager.LoadScene(MatchData.p2WinScene);

            return;
        }

        SceneManager.LoadScene(
            MatchData.gameScenes[MatchData.currentGameIndex]
        );
    }
}
