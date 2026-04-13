using System.Collections;
using UnityEngine;
using TMPro;

public class Game3Manager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text roundText;
    public TMP_Text scoreText;
    public TMP_Text finalResultText;

    [Header("玩家Animator")]
    public Animator playerAAnimator;
    public Animator playerBAnimator;

    [Header("A动画状态名")]
    public string aIdleState = "A_Idle";
    public string aUpState = "A_Up";
    public string aDownState = "A_Down";
    public string aLeftState = "A_Left";
    public string aRightState = "A_Right";

    [Header("B动画状态名")]
    public string bIdleState = "B_Idle";
    public string bUpState = "B_Up";
    public string bDownState = "B_Down";
    public string bLeftState = "B_Left";
    public string bRightState = "B_Right";

    [Header("动画时长")]
    public float aActionDuration = 0.5f;
    public float bActionDuration = 0.5f;

    [Header("设置")]
    public int maxRounds = 3;
    public int winRounds = 2;

    private int currentRound = 1;
    private int playerAScore = 0;
    private int playerBScore = 0;

    private bool canInput = false;
    private bool roundResolving = false;

    private bool playerAChosen = false;
    private bool playerBChosen = false;

    private Direction playerAChoice;
    private Direction playerBChoice;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    void Start()
    {
        if (finalResultText != null)
            finalResultText.gameObject.SetActive(false);

        if (scoreText != null)
            scoreText.transform.SetAsLastSibling();

        if (finalResultText != null)
            finalResultText.transform.SetAsLastSibling();

        ResetToIdle();
        UpdateScoreUI();

        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (!canInput || roundResolving) return;

        ReadPlayerAInput();
        ReadPlayerBInput();

        // 两个人都输入完后，再同时播动画并结算
        if (playerAChosen && playerBChosen)
        {
            canInput = false;
            roundResolving = true;
            StartCoroutine(PlayAnimationsThenResolve());
        }
    }

    IEnumerator GameLoop()
    {
        while (currentRound <= maxRounds &&
               playerAScore < winRounds &&
               playerBScore < winRounds)
        {
            if (roundText != null)
                roundText.text = $"第 {currentRound} / {maxRounds} 轮";

            ResetRoundState();
            ResetToIdle();

            yield return StartCoroutine(Countdown());

            canInput = true;

            while (!roundResolving && canInput)
                yield return null;

            while (roundResolving)
                yield return null;

            currentRound++;
        }

        ShowFinalResult();
    }

    IEnumerator Countdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        if (roundText != null)
            roundText.gameObject.SetActive(true);

        for (int i = 4; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();

            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (roundText != null)
            roundText.gameObject.SetActive(false);
    }

    void ResetRoundState()
    {
        canInput = false;
        roundResolving = false;
        playerAChosen = false;
        playerBChosen = false;
    }

    void ReadPlayerAInput()
    {
        if (playerAChosen) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            playerAChoice = Direction.Left;
            playerAChosen = true;
            Debug.Log("A选择了 ←");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            playerAChoice = Direction.Up;
            playerAChosen = true;
            Debug.Log("A选择了 ↑");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerAChoice = Direction.Down;
            playerAChosen = true;
            Debug.Log("A选择了 ↓");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            playerAChoice = Direction.Right;
            playerAChosen = true;
            Debug.Log("A选择了 →");
        }
    }

    void ReadPlayerBInput()
    {
        if (playerBChosen) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerBChoice = Direction.Left;
            playerBChosen = true;
            Debug.Log("B选择了 ←");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerBChoice = Direction.Up;
            playerBChosen = true;
            Debug.Log("B选择了 ↑");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerBChoice = Direction.Down;
            playerBChosen = true;
            Debug.Log("B选择了 ↓");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerBChoice = Direction.Right;
            playerBChosen = true;
            Debug.Log("B选择了 →");
        }
    }

    IEnumerator PlayAnimationsThenResolve()
    {
        // 两个人都输入完后，同时播放各自动画
        PlayPlayerAAnimation(playerAChoice);
        PlayPlayerBAnimation(playerBChoice);

        Debug.Log("A和B开始同时播放动画");

        // 等两个动画都播完
        float waitTime = Mathf.Max(aActionDuration, bActionDuration);
        yield return new WaitForSeconds(waitTime);

        // 再判定输赢
        ResolveRound();

        roundResolving = false;
    }

    void PlayPlayerAAnimation(Direction dir)
    {
        if (playerAAnimator == null) return;

        switch (dir)
        {
            case Direction.Up:
                playerAAnimator.Play(aUpState, 0, 0f);
                break;
            case Direction.Down:
                playerAAnimator.Play(aDownState, 0, 0f);
                break;
            case Direction.Left:
                playerAAnimator.Play(aLeftState, 0, 0f);
                break;
            case Direction.Right:
                playerAAnimator.Play(aRightState, 0, 0f);
                break;
        }
    }

    void PlayPlayerBAnimation(Direction dir)
    {
        if (playerBAnimator == null) return;

        switch (dir)
        {
            case Direction.Up:
                playerBAnimator.Play(bUpState, 0, 0f);
                break;
            case Direction.Down:
                playerBAnimator.Play(bDownState, 0, 0f);
                break;
            case Direction.Left:
                playerBAnimator.Play(bLeftState, 0, 0f);
                break;
            case Direction.Right:
                playerBAnimator.Play(bRightState, 0, 0f);
                break;
        }
    }

    void ResetToIdle()
    {
        if (playerAAnimator != null)
            playerAAnimator.Play(aIdleState, 0, 0f);

        if (playerBAnimator != null)
            playerBAnimator.Play(bIdleState, 0, 0f);
    }

    void ResolveRound()
    {
        Debug.Log("A最终选择: " + playerAChoice);
        Debug.Log("B最终选择: " + playerBChoice);

        if (playerAChoice == playerBChoice)
        {
            Debug.Log("结果：同向 → A得分");
            playerAScore++;
        }
        else
        {
            Debug.Log("结果：异向 → B得分");
            playerBScore++;
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"{playerAScore} : {playerBScore}";
    }

    void ShowFinalResult()
    {
        if (finalResultText == null) return;

        finalResultText.gameObject.SetActive(true);

        if (playerAScore >= winRounds)
            finalResultText.text = "臭咪洗澡吧！";
        else if (playerBScore >= winRounds)
            finalResultText.text = "臭咪就不洗！";
        else
            finalResultText.text = "对局结束";
    }
}