using UnityEngine;
using UnityEngine.UI;

public class SelectCardUI : MonoBehaviour
{
    public Image screenshotImage;
    public string gameSceneName;

    public void SetData(SelectCardData data)
    {
        gameSceneName = data.gameSceneName;
        screenshotImage.sprite = data.screenshot;
    }
}