using UnityEngine;
using UnityEngine.SceneManagement;

public class GameA_WinLogic : MonoBehaviour
{
    private bool ended = false; // 防止重复计分

    [Header("跳转场景")]
    public string resultBridgeSceneName = "ResultBridgeScene";

    public void Player1Win()
    {
        if (ended) return;
        ended = true;

        MatchData.player1Score++;

        GoNext();
    }

    public void Player2Win()
    {
        if (ended) return;
        ended = true;

        MatchData.player2Score++;

        GoNext();
    }

    void GoNext()
    {
        SceneManager.LoadScene(resultBridgeSceneName);
    }
}