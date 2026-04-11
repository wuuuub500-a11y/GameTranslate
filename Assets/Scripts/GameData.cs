using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameData
{
    // 玩家名字
    public static string playerAName = "";
    public static string playerBName = "";

    // 总胜场
    public static int playerAWins = 0;
    public static int playerBWins = 0;

    // 当前要进入的小游戏场景名
    public static string selectedGameScene = "";

    // 所有未玩过的小游戏
    public static List<string> remainingGames = new List<string>();

    // 已玩过的小游戏
    public static List<string> playedGames = new List<string>();

    // 初始化整局数据
    public static void InitGameData()
    {
        playerAWins = 0;
        playerBWins = 0;
        selectedGameScene = "";

        remainingGames = new List<string>
        {
            "Game1",
            "Game2",
            "Game3",
            "Game4",
            "Game5"
        };

        playedGames = new List<string>();
    }
}