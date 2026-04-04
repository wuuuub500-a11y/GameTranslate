using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game3Manager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text roundText;
    public TMP_Text scoreText;
    public TMP_Text finalResultText;

    [Header("玩家图片")]
    public Image playerAImage;
    public Image playerBImage;

    [Header("A图片")]
    public Sprite aIdleSprite;
    public Sprite aUpSprite;
    public Sprite aDownSprite;
    public Sprite aLeftSprite;
    public Sprite aRightSprite;

    [Header("B图片")]
    public Sprite bIdleSprite;
    public Sprite bUpSprite;
    public Sprite bDownSprite;
    public Sprite bLeftSprite;
    public Sprite bRightSprite;

    [Header("设置")]
    public int maxRounds = 3;
    public int winRounds = 2;
    public float afterInputDelay = 0.5f;

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
        finalResultText.gameObject.SetActive(false);

        scoreText.transform.SetAsLastSibling();
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

        if (playerAChosen && playerBChosen)
        {
            canInput = false;
            roundResolving = true;
            StartCoroutine(ResolveRound());
        }
    }

    IEnumerator GameLoop()
    {
        while (currentRound <= maxRounds &&
               playerAScore < winRounds &&
               playerBScore < winRounds)
        {
            roundText.text = $"第 {currentRound} / {maxRounds} 轮";

            ResetRoundState();
            ResetToIdle();

            yield return StartCoroutine(Countdown());

            canInput = true;

            while (roundResolving == false && canInput == true)
            {
                yield return null;
            }

            while (roundResolving)
            {
                yield return null;
            }

            currentRound++;
        }

        ShowFinalResult();
    }

    IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);
        roundText.gameObject.SetActive(true);

        for (int i = 4; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
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
            ShowPlayerA(Direction.Left);
            Debug.Log("A按了 ←");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            playerAChoice = Direction.Up;
            playerAChosen = true;
            ShowPlayerA(Direction.Up);
            Debug.Log("A按了 ↑");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerAChoice = Direction.Down;
            playerAChosen = true;
            ShowPlayerA(Direction.Down);
            Debug.Log("A按了 ↓");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            playerAChoice = Direction.Right;
            playerAChosen = true;
            ShowPlayerA(Direction.Right);
            Debug.Log("A按了 →");
        }
    }

    void ReadPlayerBInput()
    {
        if (playerBChosen) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerBChoice = Direction.Left;
            playerBChosen = true;
            ShowPlayerB(Direction.Left);
            Debug.Log("B按了 ←");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerBChoice = Direction.Up;
            playerBChosen = true;
            ShowPlayerB(Direction.Up);
            Debug.Log("B按了 ↑");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerBChoice = Direction.Down;
            playerBChosen = true;
            ShowPlayerB(Direction.Down);
            Debug.Log("B按了 ↓");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerBChoice = Direction.Right;
            playerBChosen = true;
            ShowPlayerB(Direction.Right);
            Debug.Log("B按了 →");
        }
    }

    void ShowPlayerA(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                playerAImage.sprite = aUpSprite;
                break;
            case Direction.Down:
                playerAImage.sprite = aDownSprite;
                break;
            case Direction.Left:
                playerAImage.sprite = aLeftSprite;
                break;
            case Direction.Right:
                playerAImage.sprite = aRightSprite;
                break;
        }

        PopEffect(playerAImage.transform);
    }

    void ShowPlayerB(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                playerBImage.sprite = bUpSprite;
                break;
            case Direction.Down:
                playerBImage.sprite = bDownSprite;
                break;
            case Direction.Left:
                playerBImage.sprite = bLeftSprite;
                break;
            case Direction.Right:
                playerBImage.sprite = bRightSprite;
                break;
        }

        PopEffect(playerBImage.transform);
    }

    void ResetToIdle()
    {
        playerAImage.sprite = aIdleSprite;
        playerBImage.sprite = bIdleSprite;

        playerAImage.transform.localScale = Vector3.one;
        playerBImage.transform.localScale = Vector3.one;
    }

    void PopEffect(Transform t)
    {
        t.localScale = Vector3.one * 1.2f;
        StartCoroutine(ScaleBack(t));
    }

    IEnumerator ScaleBack(Transform t)
    {
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;
    }

    IEnumerator ResolveRound()
    {
        yield return new WaitForSeconds(afterInputDelay);

        // 双保险：没有双方输入就不结算
        if (!playerAChosen || !playerBChosen)
        {
            Debug.LogWarning("本轮未完成输入，不进行结算");
            roundResolving = false;
            yield break;
        }

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

        roundResolving = false;
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"{playerAScore} : {playerBScore}";
        Debug.Log("当前比分: " + scoreText.text);
    }

    void ShowFinalResult()
    {
        finalResultText.gameObject.SetActive(true);

        if (playerAScore >= winRounds)
            finalResultText.text = "臭咪洗澡吧！";
        else if (playerBScore >= winRounds)
            finalResultText.text = "臭咪就不洗！";
        else
            finalResultText.text = "对局结束";
    }
}