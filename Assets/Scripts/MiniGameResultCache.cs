using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public static class MiniGameResultCache
{
    // 0 = Œ¥…Ë÷√
    // 1 = A”Æ
    // 2 = B”Æ
    public static int winner = 0;

    public static string lastGameScene = "";

    public static void Clear()
    {
        winner = 0;
        lastGameScene = "";
    }
}