using UnityEngine;
using TMPro;

public class SequenceUI : MonoBehaviour
{
    public PlayerInput player;

    // 每轮对应的槽位序列（直接在场景中手动拖入，手动调整位置）
    public TextMeshProUGUI[] slotRound1;
    public TextMeshProUGUI[] slotRound2;
    public TextMeshProUGUI[] slotRound3;
    public TextMeshProUGUI[] slotRound4;
    public TextMeshProUGUI[] slotRound5;

    public Color normalColor = Color.gray;
    public Color correctColor = Color.green;
    public Color currentColor = Color.white;

    // 弹跳动画参数
    public float bounceSpeed = 8f;
    public float bounceScale = 0.2f;

    private TextMeshProUGUI[] currentSlots;
    private int currentRound;
    private string[] currentSequenceTexts;

    public void SetRound(int round)
    {
        currentRound = Mathf.Clamp(round, 1, 5);
        switch (currentRound)
        {
            case 1: currentSlots = slotRound1; break;
            case 2: currentSlots = slotRound2; break;
            case 3: currentSlots = slotRound3; break;
            case 4: currentSlots = slotRound4; break;
            case 5: currentSlots = slotRound5; break;
        }
    }

    public void SetSequenceTexts(string[] texts)
    {
        currentSequenceTexts = texts;
    }

    void Update()
    {
        if (player.targetSequence == null || currentSlots == null || currentSequenceTexts == null) return;

        for (int i = 0; i < currentSlots.Length; i++)
        {
            if (currentSlots[i] == null) continue;

            // 超出目标序列长度的槽位隐藏
            if (i >= player.targetSequence.Length)
            {
                if (currentSlots[i].gameObject.activeSelf)
                    currentSlots[i].gameObject.SetActive(false);
                continue;
            }

            // 显示槽位
            if (!currentSlots[i].gameObject.activeSelf)
                currentSlots[i].gameObject.SetActive(true);

            // 显示对应的字符
            currentSlots[i].text = currentSequenceTexts[i];

            // 根据玩家输入状态设置槽位颜色和动画
            if (i < player.currentIndex)
            {
                // 已正确输入的：绿色，无动画
                currentSlots[i].color = correctColor;
                currentSlots[i].transform.localScale = Vector3.one;
            }
            else if (i == player.currentIndex)
            {
                // 当前输入：高亮白色 + 弹跳动画
                currentSlots[i].color = currentColor;
                float scale = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceScale;
                currentSlots[i].transform.localScale = Vector3.one * scale;
            }
            else
            {
                // 未输入的：灰色，无动画
                currentSlots[i].color = normalColor;
                currentSlots[i].transform.localScale = Vector3.one;
            }
        }
    }
}