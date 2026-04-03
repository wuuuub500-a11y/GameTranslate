using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerHuman human;
    public PlayerMouse mouse;

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

    void EndGame()
    {
        Debug.Log("游戏结束");

        if (humanScore > mouseScore)
            Debug.Log("人赢！");
        else if (mouseScore > humanScore)
            Debug.Log("鼠赢！");
        else
            Debug.Log("平局！");
    }
}