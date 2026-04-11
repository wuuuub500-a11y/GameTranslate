using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCarouselManager : MonoBehaviour
{
    [Header("槽位")]
    public RectTransform[] slots; // 5个槽位，按左到右顺序拖入

    [Header("卡牌")]
    public SelectCardUI[] cards; // 5张卡牌

    [Header("轮播数据")]
    public SelectCardData[] allGames; // Game1~Game5 的截图和场景名

    [Header("动画参数")]
    public float moveDuration = 0.15f;
    public float stopScaleDuration = 0.25f;
    public int minShuffleSteps = 12;
    public int maxShuffleSteps = 20;

    [Header("停止后等待进入游戏时间")]
    public float enterDelay = 0.8f;

    private List<SelectCardUI> activeCards = new List<SelectCardUI>();
    private bool isRolling = false;

    private void Start()
    {
        if (GameData.remainingGames == null || GameData.remainingGames.Count == 0)
        {
            Debug.Log("检测到 remainingGames 为空，自动初始化测试数据");
            GameData.InitGameData();
        }

        InitCards();
        ApplyLayoutInstant();
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
        Debug.Log("点击了 StartRollToRandomGame");
        if (isRolling) return;

        if (GameData.remainingGames.Count == 0)
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

            // 后期稍微减速
            if (i > totalSteps * 0.6f)
                yield return new WaitForSeconds(0.03f * (i - totalSteps * 0.6f + 1));
        }

        // 调整到目标卡牌在中间
        int safeCount = 20;
        while (GetCenterCard().gameSceneName != targetScene && safeCount > 0)
        {
            ShiftLeft();
            yield return StartCoroutine(AnimateToSlots());
            safeCount--;
        }

        // 中间卡牌再弹一下
        yield return StartCoroutine(PunchCenterCard());

        // 记录本次选择
        GameData.selectedGameScene = targetScene;
        GameData.remainingGames.Remove(targetScene);
        GameData.playedGames.Add(targetScene);

        yield return new WaitForSeconds(enterDelay);

        SceneManager.LoadScene(targetScene);
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
            RectTransform cardRect = activeCards[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = slots[i].anchoredPosition;
            cardRect.localScale = slots[i].localScale;

            Canvas canvas = activeCards[i].GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = GetSortOrderBySlot(i);
            }
        }

        // 中间卡永远在最上层
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
            case 2: return 100; // 中间最大，永远最上
            case 1: return 80;
            case 3: return 80;
            case 0: return 60;
            case 4: return 60;
            default: return 0;
        }
    }
    float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }
}