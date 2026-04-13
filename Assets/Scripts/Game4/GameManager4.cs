using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 


public class GameManager4: MonoBehaviour
{
    public GameA_WinLogic winLogic;
    public PlayerHuman human;
    public PlayerMouse mouse;
    public TextMeshProUGUI countdownText;

    private bool ended = false;//用来防止重复计分的 需要加这个变量

    public int humanScore = 0;
    public int mouseScore = 0;

    public float decisionTime = 3f;
    public float animationTime = 1.5f;
    public float totalTime = 45f;

    private float timer;

    // 记录上一次选择
    private LegPosition lastHumanChoice = LegPosition.TopLeft;
    private LegPosition lastMouseChoice = LegPosition.TopLeft;

    // Table物体的引用
    public Transform tableTransform;
    public Transform mouseTransform;

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
        /*if (tableTransform != null)
        {
            Vector3 resetPos = tableTransform.position;
            resetPos.x = 4.44f;
            tableTransform.position = resetPos;
        }*/
        human.ResetChoice();
        mouse.ResetChoice();

        int count = Mathf.CeilToInt(decisionTime);

        while (count > 0)
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = count.ToString();
            }

            yield return new WaitForSeconds(1f);
            count--;
        }

        // 倒计时结束 → 隐藏
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        LegPosition humanChoice = human.hasChosen ? human.currentChoice : lastHumanChoice;
        LegPosition mouseChoice = mouse.hasChosen ? mouse.currentChoice : lastMouseChoice;

        lastHumanChoice = humanChoice;
        lastMouseChoice = mouseChoice;

        Debug.Log("Human: " + humanChoice + " | Mouse: " + mouseChoice);

        bool caught = this.Resolve(humanChoice, mouseChoice);

        // ========== 触发右手抓的动画（根据人的选择）==========
        string grabType = GetGrabType(humanChoice);
        if (grabType == "L")
        {
            human.GetComponent<Animator>().SetTrigger("LGrab");
        }
        else
        {
            human.GetComponent<Animator>().SetTrigger("RGrab");
        }

        // ========== 老鼠动画：只有被抓才惊讶 ==========
        if (caught)
        {
            mouse.GetComponent<Animator>().SetTrigger("Surprised");
            Debug.Log("抓到了！");
            humanScore++;
        }
        else
        {
            Debug.Log("逃掉了！");
            mouseScore++;
        }

        
        MoveTable(mouseChoice);

        // 打印当前比分
        Debug.Log($"当前比分 人:{humanScore} 鼠:{mouseScore}");

        // 等待动画播放完
        yield return new WaitForSeconds(animationTime);
    }

    public bool Resolve(LegPosition human, LegPosition mouse)
    {
        return human == mouse;
    }

    string GetGrabType(LegPosition pos)
    {
        switch (pos)
        {
            case LegPosition.TopLeft:
            case LegPosition.BottomLeft:
                return "L";  // 左边 → LGrab
            case LegPosition.TopRight:
            case LegPosition.BottomRight:
                return "R";  // 右边 → RGrab
        }
        return "L";
    }

    // 根据老鼠选择的床脚移动Table
    void MoveTable(LegPosition mouseChoice)
    {
        if (tableTransform == null)
        {
            Debug.LogWarning("Table Transform未设置！");
            return;
        }

        Vector3 newPosT = tableTransform.position;
        Vector3 newPosM= mouseTransform.position;

        switch (mouseChoice)
        {
            case LegPosition.TopLeft:
            case LegPosition.BottomLeft:
                // 左边床脚：x = 8.88
                newPosT.x = 8.88f;
                Debug.Log("老鼠选择左边，Table移动到 x = 8.88");
                break;
            case LegPosition.TopRight:
            case LegPosition.BottomRight:
                // 右边床脚：x = 0.24
                newPosT.x = 0.24f;
                Debug.Log("老鼠选择右边，Table移动到 x = 0.24");
                break;
        }


        switch (mouseChoice)
        {
            case LegPosition.TopLeft:
            case LegPosition.TopRight:
                
                newPosM.x = 2.45f;
                Debug.Log("老鼠选择上边，老鼠2.45");
                break;
            case LegPosition.BottomLeft:
            case LegPosition.BottomRight:
                
                newPosM.x = 6.88f;
                Debug.Log("老鼠选择下边，老鼠6.88");
                break;
        }
        tableTransform.position = newPosT;
        mouseTransform.position = newPosM;
    }

    void EndGame()
    {
        if (ended) return;
        ended = true;
        Debug.Log("游戏结束！最终比分 人:" + humanScore + " 鼠:" + mouseScore);

        if (humanScore > mouseScore)
        {
            Debug.Log("人赢");
            winLogic.Player1Win();
        }
        else if (mouseScore > humanScore)
        {
            Debug.Log("鼠赢");
            winLogic.Player2Win();
        }
        else
        {
            Debug.Log("平局");
            SceneManager.LoadScene("ResultBridgeScene");
        }
    }

   

    void GoNext()
    {
        SceneManager.LoadScene("ResultBridgeScene");
    }
}