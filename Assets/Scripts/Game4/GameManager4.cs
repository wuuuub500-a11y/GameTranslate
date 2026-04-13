using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerHuman human;
    public PlayerMouse mouse;

    private bool ended = false;//用来防止重复计分的 需要加这个变量

    public GameObject resultUI;


    public int humanScore = 0;
    public int mouseScore = 0;

    public float decisionTime = 3f;  
    public float animationTime = 1.5f; 
    public float totalTime = 45f;

    private float timer;

    // 记录上一次选择
    private LegPosition lastHumanChoice = LegPosition.TopLeft;
    private LegPosition lastMouseChoice = LegPosition.TopLeft;

    void Start()
    {
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        timer = totalTime;

        while (timer > 0)
        {
            yield return StartCoroutine(PlayRound());
            timer -= (decisionTime + animationTime);
        }

        EndGame();
    }

    IEnumerator PlayRound()
    {
        resultUI.SetActive(false);

        human.ResetChoice();
        mouse.ResetChoice();

       
        float t = decisionTime;

        while (t > 0)
        {
            
            yield return new WaitForSeconds(1f);
            t -= 1f;
        }

       
        LegPosition humanChoice = human.hasChosen ? human.currentChoice : lastHumanChoice;
        LegPosition mouseChoice = mouse.hasChosen ? mouse.currentChoice : lastMouseChoice;

        
        lastHumanChoice = humanChoice;
        lastMouseChoice = mouseChoice;

        Debug.Log("Human: " + humanChoice + " | Mouse: " + mouseChoice);

        
        bool caught =this.Resolve(humanChoice, mouseChoice);

        if (caught)
        {
            humanScore++;
            Debug.Log("抓到了！");
            
            
            resultUI.SetActive(true);
            resultUI.GetComponent<Animator>().SetTrigger("HumanWin");
        }
        else
        {
            mouseScore++;
            Debug.Log("逃掉了！");
            
            resultUI.SetActive(true);
            resultUI.GetComponent<Animator>().SetTrigger("MouseWin");
        }

        
        yield return new WaitForSeconds(animationTime);
    }

    public bool Resolve(LegPosition human, LegPosition mouse)
    {
        return human == mouse; 
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
    



    void EndGame()
    {
        if (ended) return;
        ended = true;
        Debug.Log("游戏结束");

        if (humanScore > mouseScore)
        {
            Debug.Log("人赢！");
            Player1Win();
        }


        else if (mouseScore > humanScore)
        {
            Debug.Log("鼠赢！");
            Player2Win();
        }



        else
        { 
            Debug.Log("平局！");
            GoNext();
        }
            
    }
}