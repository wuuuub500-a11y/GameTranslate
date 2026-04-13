using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public string aUpState = "A_up";
    public string aDownState = "A_down";
    public string aLeftState = "A_left";
    public string aRightState = "A_right";

    [Header("B动画状态名")]
    public string bIdleState = "B_Idle";
    public string bUpState = "B_Up";
    public string bDownState = "B_Down";
    public string bLeftState = "B_Left";
    public string bRightState = "B_Right";

    [Header("动画时长")]
    public float aActionDuration = 0.5f;
    public float bActionDuration = 0.5f;

    [Header("音效")]
    public AudioSource inputAudioSource;
    public AudioSource roundResultAudioSource;

    public AudioClip playerAInputSfx;
    public AudioClip playerBInputSfx;

    public AudioClip playerAWinSfx;
    public AudioClip playerBWinSfx;

    [Header("结算跳转")]
    public string nextSceneName = "ResultBridgeScene";
    public float endStayDuration = 4f;

    [Header("设置")]
    public int maxRounds = 3;
    public int winRounds = 2;

    private int currentRound = 1;
    private int playerAScore = 0;
    private int playerBScore = 0;

    private bool canInput = false;
    private bool roundResolving = false;
    private bool gameEnded = false;

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

        ResetToIdle();
        UpdateScoreUI();

        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (!canInput || roundResolving || gameEnded) return;

        ReadPlayerAInput();
        ReadPlayerBInput();

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

        yield return StartCoroutine(EndGame());
    }

    IEnumerator Countdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
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
            PlayInputSfx(playerAInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            playerAChoice = Direction.Up;
            playerAChosen = true;
            PlayInputSfx(playerAInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerAChoice = Direction.Down;
            playerAChosen = true;
            PlayInputSfx(playerAInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            playerAChoice = Direction.Right;
            playerAChosen = true;
            PlayInputSfx(playerAInputSfx);
        }
    }

    void ReadPlayerBInput()
    {
        if (playerBChosen) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerBChoice = Direction.Left;
            playerBChosen = true;
            PlayInputSfx(playerBInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerBChoice = Direction.Up;
            playerBChosen = true;
            PlayInputSfx(playerBInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerBChoice = Direction.Down;
            playerBChosen = true;
            PlayInputSfx(playerBInputSfx);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerBChoice = Direction.Right;
            playerBChosen = true;
            PlayInputSfx(playerBInputSfx);
        }
    }

    IEnumerator PlayAnimationsThenResolve()
    {
        PlayPlayerAAnimation(playerAChoice);
        PlayPlayerBAnimation(playerBChoice);

        float waitTime = Mathf.Max(aActionDuration, bActionDuration);
        yield return new WaitForSeconds(waitTime);

        ResolveRound();

        roundResolving = false;
    }

    void PlayPlayerAAnimation(Direction dir)
    {
        if (playerAAnimator == null) return;

        playerAAnimator.Play(GetAState(dir), 0, 0f);
    }

    void PlayPlayerBAnimation(Direction dir)
    {
        if (playerBAnimator == null) return;

        playerBAnimator.Play(GetBState(dir), 0, 0f);
    }

    string GetAState(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return aUpState;
            case Direction.Down: return aDownState;
            case Direction.Left: return aLeftState;
            case Direction.Right: return aRightState;
        }
        return aIdleState;
    }

    string GetBState(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return bUpState;
            case Direction.Down: return bDownState;
            case Direction.Left: return bLeftState;
            case Direction.Right: return bRightState;
        }
        return bIdleState;
    }

    void ResetToIdle()
    {
        if (playerAAnimator != null)
            playerAAnimator.Play(aIdleState);

        if (playerBAnimator != null)
            playerBAnimator.Play(bIdleState);
    }

    void ResolveRound()
    {
        if (playerAChoice == playerBChoice)
        {
            playerAScore++;
            PlayRoundResultSfx(playerAWinSfx);
        }
        else
        {
            playerBScore++;
            PlayRoundResultSfx(playerBWinSfx);
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"{playerAScore} : {playerBScore}";
    }

    IEnumerator EndGame()
    {
        gameEnded = true;

        // ⭐⭐⭐ 核心：写入总比分 ⭐⭐⭐
        if (playerAScore >= winRounds)
        {
            GameData.playerAWins++;
        }
        else
        {
            GameData.playerBWins++;
        }

        if (finalResultText != null)
        {
            finalResultText.gameObject.SetActive(true);

            if (playerAScore >= winRounds)
                finalResultText.text = "洗澡吧臭咪!";
            else
                finalResultText.text = "臭咪就不洗!";
        }

        yield return new WaitForSeconds(endStayDuration);

        SceneManager.LoadScene(nextSceneName);
    }

    void PlayInputSfx(AudioClip clip)
    {
        if (inputAudioSource != null && clip != null)
            inputAudioSource.PlayOneShot(clip);
    }

    void PlayRoundResultSfx(AudioClip clip)
    {
        if (roundResultAudioSource != null && clip != null)
            roundResultAudioSource.PlayOneShot(clip);
    }
}