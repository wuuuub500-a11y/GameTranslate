using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        float t = GameManager_1.Instance.GetTime();
        text.text = Mathf.CeilToInt(t).ToString();
    }
}
