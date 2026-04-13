using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCarouselManager : MonoBehaviour
{
    [Header("槽位")]
    public RectTransform[] slots;

    [Header("卡牌")]
    public SelectCardUI[] cards;

    [Header("轮播数据")]
    public SelectCardData[] allGames;

    [Header("动画参数")]
    public float moveDuration = 0.15f;
    public float stopScaleDuration = 0.25f;
    public int minShuffleSteps = 12;
    public int maxShuffleSteps = 20;

    [Header("玩法展示UI")]
    public Image ruleImageDisplay;
    public GameObject startGameButton;

    [Header("轮播阶段UI")]
    public GameObject carouselRoot;
    public GameObject startSelectButton;

    [Header("规则阶段UI")]
    public Image RuleImageDisplay;
    public GameObject StartGameButton;

    private List<SelectCardUI> activeCards = new List<SelectCardUI>();
    private bool isRolling = false;
    private string selectedSceneName = "";

    private void Start()
    {
        if (GameData.remainingGames == null || GameData.remainingGames.Count == 0)
        {
            Debug.Log("remainingGames 为空，自动初始化测试数据");
            GameData.InitGameData();
        }

        InitCards();
        ApplyLayoutInstant();

        // 初始：轮播UI显示
        if (carouselRoot != null)
            carouselRoot.SetActive(true);

        if (startSelectButton != null)
            startSelectButton.SetActive(true);

        // 初始：规则UI隐藏
        if (ruleImageDisplay != null)
            ruleImageDisplay.gameObject.SetActive(false);

        if (startGameButton != null)
            startGameButton.SetActive(false);
    }

    void InitCards()
    {
        activeCards.Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetData(allGames[i]);
            activeCards.Add(cards[i]);
        }
    }

    public void StartRollToRandomGame()
    {
        if (isRolling) return;

        if (GameData.remainingGames == null || GameData.remainingGames.Count == 0)
        {
            Debug.Log("没有剩余游戏可选");
            return;
        }

        string targetScene = GetRandomRemainingGame();
        StartCoroutine(RollRoutine(targetScene));
    }

    string GetRandomRemainingGame()
    {
        int index = Random.Range(0, GameData.remainingGames.Count);
        return GameData.remainingGames[index];
    }

    IEnumerator RollRoutine(string targetScene)
    {
        isRolling = true;

        int totalSteps = Random.Range(minShuffleSteps, maxShuffleSteps + 1);

        for (int i = 0; i < totalSteps; i++)
        {
            ShiftLeft();
            yield return StartCoroutine(AnimateToSlots());

            if (i > totalSteps * 0.6f)
                yield return new WaitForSeconds(0.03f * (i - (int)(totalSteps * 0.6f) + 1));
        }

        int safeCount = 20;
        while (GetCenterCard().gameSceneName != targetScene && safeCount > 0)
        {
            ShiftLeft();
            yield return StartCoroutine(AnimateToSlots());
            safeCount--;
        }

        yield return StartCoroutine(PunchCenterCard());

        // ⭐ 这里只记录，不直接进游戏
        selectedSceneName = targetScene;

        // 从 remainingGames 中移除，避免重复
        if (GameData.remainingGames.Contains(targetScene))
            GameData.remainingGames.Remove(targetScene);

        if (!GameData.playedGames.Contains(targetScene))
            GameData.playedGames.Add(targetScene);

        // 显示玩法图和 Start 按钮
        ShowRulePanel(targetScene);

        isRolling = false;
    }

    void ShowRulePanel(string sceneName)
    {
        SelectCardData data = GetDataBySceneName(sceneName);
        if (data == null) return;

        // 关闭轮播阶段UI
        if (carouselRoot != null)
            carouselRoot.SetActive(false);

        if (startSelectButton != null)
            startSelectButton.SetActive(false);

        // 打开规则阶段UI
        if (ruleImageDisplay != null)
        {
            ruleImageDisplay.sprite = data.ruleImage;
            ruleImageDisplay.gameObject.SetActive(true);
            ruleImageDisplay.transform.SetAsLastSibling();
        }

        if (startGameButton != null)
        {
            startGameButton.SetActive(true);
            startGameButton.transform.SetAsLastSibling();
        }
    }
    SelectCardData GetDataBySceneName(string sceneName)
    {
        foreach (var data in allGames)
        {
            if (data.gameSceneName == sceneName)
                return data;
        }
        return null;
    }

    public void StartSelectedGame()
    {
        if (string.IsNullOrEmpty(selectedSceneName))
        {
            Debug.LogWarning("还没有选中任何小游戏");
            return;
        }

        SceneManager.LoadScene(selectedSceneName);
    }

    void ShiftLeft()
    {
        SelectCardUI first = activeCards[0];
        activeCards.RemoveAt(0);
        activeCards.Add(first);
    }

    SelectCardUI GetCenterCard()
    {
        return activeCards[2];
    }

    void ApplyLayoutInstant()
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            RectTransform rect = activeCards[i].GetComponent<RectTransform>();
            rect.anchoredPosition = slots[i].anchoredPosition;
            rect.localScale = slots[i].localScale;

            Canvas canvas = activeCards[i].GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = GetSortOrderBySlot(i);
            }
        }

        activeCards[2].transform.SetAsLastSibling();
    }

    IEnumerator AnimateToSlots()
    {
        List<Vector2> startPositions = new List<Vector2>();
        List<Vector3> startScales = new List<Vector3>();

        for (int i = 0; i < activeCards.Count; i++)
        {
            RectTransform rect = activeCards[i].GetComponent<RectTransform>();
            startPositions.Add(rect.anchoredPosition);
            startScales.Add(rect.localScale);
        }

        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;
            t = EaseOutCubic(t);

            for (int i = 0; i < activeCards.Count; i++)
            {
                RectTransform rect = activeCards[i].GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(startPositions[i], slots[i].anchoredPosition, t);
                rect.localScale = Vector3.Lerp(startScales[i], slots[i].localScale, t);

                Canvas canvas = activeCards[i].GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = GetSortOrderBySlot(i);
                }
            }

            yield return null;
        }

        ApplyLayoutInstant();
        activeCards[2].transform.SetAsLastSibling();
    }

    IEnumerator PunchCenterCard()
    {
        SelectCardUI centerCard = GetCenterCard();
        RectTransform rect = centerCard.GetComponent<RectTransform>();

        Vector3 originalScale = rect.localScale;
        Vector3 targetScale = originalScale * 1.12f;

        float time = 0f;
        while (time < stopScaleDuration)
        {
            time += Time.deltaTime;
            float t = time / stopScaleDuration;
            rect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        time = 0f;
        while (time < stopScaleDuration)
        {
            time += Time.deltaTime;
            float t = time / stopScaleDuration;
            rect.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        rect.localScale = originalScale;
    }

    int GetSortOrderBySlot(int slotIndex)
    {
        switch (slotIndex)
        {
            case 2: return 100;
            case 1:
            case 3: return 80;
            case 0:
            case 4: return 60;
            default: return 0;
        }
    }

    float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }
}