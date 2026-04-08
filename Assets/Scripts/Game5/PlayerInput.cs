using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    
    //存随机生成好的目标序列
    public KeyCode[] targetSequence;
    public int currentIndex = 0;

    public bool finished = false;

    public void SetSequence(KeyCode[] seq)
    {
        targetSequence = seq;
        currentIndex = 0;
        finished = false;
    }

    void Update()
    {
        if (finished || targetSequence == null) return;

        if (Input.anyKeyDown)
        {
            //比对输入是否与目标序列的当前按键匹配，输错忽略继续等下一次输入
            if (Input.GetKeyDown(targetSequence[currentIndex]))
            {
                currentIndex++;
                if (currentIndex >= targetSequence.Length)
                {
                    finished = true;
                }
            }
        }
    }
}