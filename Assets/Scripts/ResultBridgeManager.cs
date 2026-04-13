using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultBridgeManager : MonoBehaviour
{
    [Header("停留时间")]
    public float stayDuration = 3f;

    [Header("场景名")]
    public string selectSceneName = "SelectScene";
    public string finalResultSceneName = "FinalResultScene";

    private void Start()
    {
        StartCoroutine(BridgeFlow());
    }

    IEnumerator BridgeFlow()
    {
        // 纯 loading 停留
        yield return new WaitForSeconds(stayDuration);

        // ⭐ 核心判断
        if (GameData.remainingGames == null || GameData.remainingGames.Count == 0)
        {
            Debug.Log("所有游戏已完成 → 进入 FinalResultScene");
            SceneManager.LoadScene(finalResultSceneName);
        }
        else
        {
            Debug.Log("还有剩余游戏 → 返回 SelectScene");
            SceneManager.LoadScene(selectSceneName);
        }
    }
}