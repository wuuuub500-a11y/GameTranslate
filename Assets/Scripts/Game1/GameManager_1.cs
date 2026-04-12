using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_1 : MonoBehaviour
{
    public static GameManager_1 Instance;

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
            Debug.Log("Player2 胜利！");
        }
        else
        {
            Debug.Log("Player1 胜利！");
        }
    }

    // 给UI用
    public float GetTime()
    {
        return timer;
    }
}
