
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager5 : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    public SequenceUI player1UI;
    public SequenceUI player2UI;

    public Transform objectTransform;   // 被推物体的位置
    public Transform humanSidePos;      
    public Transform catSidePos;

    private int humanScore = 0;
    private int catScore = 0;


    public int totalRounds = 5;

    private bool ended = false;//用来防止重复计分的 需要加这个变量


    public KeyCode[] p1Pool1;
    public KeyCode[] p1Pool2;
    public KeyCode[] p1Pool3;
    public KeyCode[] p1Pool4;
    public KeyCode[] p1Pool5;

    public KeyCode[] p2Pool1;
    public KeyCode[] p2Pool2;
    public KeyCode[] p2Pool3;
    public KeyCode[] p2Pool4;
    public KeyCode[] p2Pool5;
    
    void Start()
    {
        humanScore = 0;
        catScore = 0;
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        for (int round = 1; round <= totalRounds; round++)
        {
          
            yield return StartCoroutine(PlayRound(round));
        }


        Debug.Log("游戏结束");
        EndGame();
    }

    IEnumerator PlayRound(int round)
    {

        ended= false;
        // 更新UI显示当前轮次
        player1UI.SetRound(round);
        player2UI.SetRound(round);

        while (true)
        {
            //猫的5个库
            KeyCode[][] p1Pools = new KeyCode[][]
            {
            p1Pool1, p1Pool2, p1Pool3, p1Pool4, p1Pool5
            };

            //人的5个库
            KeyCode[][] p2Pools = new KeyCode[][]
            {
            p2Pool1, p2Pool2, p2Pool3, p2Pool4, p2Pool5
            };


            //按轮数用不同的库生成对应长度的序列
            KeyCode[] seq1 = GenerateSequence(round, p1Pools);
            KeyCode[] seq2 = GenerateSequence(round, p2Pools);

            player1.SetSequence(seq1);
            player2.SetSequence(seq2);

            Debug.Log("P1: " + string.Join(",", seq1));
            Debug.Log("P2: " + string.Join(",", seq2));

            while (true)
            {
                if (player1.targetSequence == null || player2.targetSequence == null)
                {
                    yield return null;
                    continue;
                }

                // 用差值算物体被移动到的位置
                int diff = player1.currentIndex - player2.currentIndex;


                float t = Mathf.Clamp01((diff + round) / (2f * round));


                objectTransform.position = Vector3.Lerp(humanSidePos.position, catSidePos.position, t);



                //判定输赢
                int result = Resolve(player1, player2);

                if (result == 1)
                {
                    Debug.Log("人赢");
                    humanScore++;
                    yield return new WaitForSeconds(1f);
                    yield break;
                }
                else if (result == 2)
                {
                    Debug.Log("猫赢");
                    catScore++;
                    yield return new WaitForSeconds(1f);
                    yield break;
                }
                else if (result == 0)
                {
                    Debug.Log("平局，重开");
                    
                    yield return new WaitForSeconds(1f);
                    break; // 重开
                }

                yield return null;
            }
        }
    }


    void EndGame()
    {
        if (ended) return;
        ended = true;

        Debug.Log($"最终比分：人 {humanScore} : 猫 {catScore}");

        if (humanScore > catScore)
        {
            Debug.Log("人最终胜利！");
            Player1Win();   
        }
        else if (catScore > humanScore)
        {
            Debug.Log("猫最终胜利！");
            Player2Win();
        }
        else
        {
            Debug.Log("平局！");
            GoNext();
        }
    }

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



    //判定谁先输入完谁赢
    public int Resolve(PlayerInput p1, PlayerInput p2)
    {
        if (p1.finished && !p2.finished) return 1;
        if (!p1.finished && p2.finished) return 2;
        if (p1.finished && p2.finished) return 0; // 平局
        return -1; // 还没结束
    }

    //按轮数选择相应的库生成相应长度的序列
    KeyCode[] GenerateSequence(int length, KeyCode[][] pools)
    {
        KeyCode[] seq = new KeyCode[length];

        for (int i = 0; i < length; i++)
        {
            KeyCode[] pool = pools[i]; // 第i个字母用第i个库
            seq[i] = pool[Random.Range(0, pool.Length)];
        }

        return seq;
    }
}