using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("NameScene");
    }
    public void OnClickExit()
    {
        Debug.Log("ÍË³öÓÎÏ·");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}