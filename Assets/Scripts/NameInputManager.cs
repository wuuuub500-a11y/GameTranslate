using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NameInputManager : MonoBehaviour
{
    [Header("两个输入框")]
    public TMP_InputField playerAInput;
    public TMP_InputField playerBInput;

    [Header("下一个场景名")]
    public string nextSceneName = "SelectScene";

    private void Start()
    {
        if (playerAInput != null)
        {
            playerAInput.ActivateInputField();
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            TryEnterNextScene();
        }
    }

    private void TryEnterNextScene()
    {
        string playerAName = playerAInput.text.Trim();
        string playerBName = playerBInput.text.Trim();

        if (!string.IsNullOrEmpty(playerAName) && !string.IsNullOrEmpty(playerBName))
        {

            PlayerData.playerAName = playerAName;
            PlayerData.playerBName = playerBName;

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("请让两位玩家都输入用户名后再按 Enter");
        }
    }
}