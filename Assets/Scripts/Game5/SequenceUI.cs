using UnityEngine;
using UnityEngine.UI;

public class SequenceUI : MonoBehaviour
{
    public PlayerInput player;

    // 每轮对应的槽位序列
    public Image[] slotRound1;
    public Image[] slotRound2;
    public Image[] slotRound3;
    public Image[] slotRound4;
    public Image[] slotRound5;

   
    public Color normalColor = Color.gray;
    public Color correctColor = Color.green;
    public Color currentColor = Color.white;

    private Image[] currentSlots;  // 当前轮使用的槽位数组
    private int currentRound;  

    // 提供给 GameManager 更新轮次
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

    void Update()
    {
        if (player.targetSequence == null || currentSlots == null) return;

        for (int i = 0; i < currentSlots.Length; i++)
        {

            // 超出目标序列长度的槽位隐藏
            if (i >= player.targetSequence.Length)
            {
                currentSlots[i].gameObject.SetActive(false);
                continue;
            }

            //显示每轮不同的序列
            currentSlots[i].gameObject.SetActive(true);

            // 根据玩家输入状态设置槽位颜色和动画
            if (i < player.currentIndex)
            {
                // 已正确输入的彩色
                currentSlots[i].color = correctColor;
                currentSlots[i].transform.localScale = Vector3.one;
            }
            else if (i == player.currentIndex)
            {
                // 当前输入：高亮 + 弹跳
                currentSlots[i].color = currentColor;
                float scale = 1f + Mathf.Sin(Time.time * 8f) * 0.2f;
                currentSlots[i].transform.localScale = Vector3.one * scale;
            }
            else
            {
                // 未输入的灰色
                currentSlots[i].color = normalColor;
                currentSlots[i].transform.localScale = Vector3.one;
            }
        }
    }
}