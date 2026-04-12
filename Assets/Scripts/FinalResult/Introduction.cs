using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*public class GameA_WinLogic : MonoBehaviour
{
    void Player1Win()
    {
        // 原本逻辑
    }

    void Player2Win()
    {
        // 原本逻辑
    }
}*/

//将你做的游戏场景里用来判断胜负的代码部分（类似上方的逻辑）改为以下

public class GameA_WinLogic : MonoBehaviour
{
    private bool ended = false;//用来防止重复计分的 需要加这个变量

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


