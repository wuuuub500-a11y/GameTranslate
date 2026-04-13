using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager5 : MonoBehaviour
{
    public GameA_WinLogic winLogic;

    public PlayerInput player1;
    public PlayerInput player2;

    public SequenceUI player1UI;
    public SequenceUI player2UI;

    public Transform humanSidePos;
    public Transform catSidePos;

    public Transform[] objectTransforms;
    private Transform currentObject;

    private Vector3[] objectStartPositions;

    public Animator humanAnimator;
    public Animator catAnimator;

    private int humanScore = 0;
    private int catScore = 0;

    public int totalRounds = 5;
    private bool ended = false;

    public string[] p1Round1, p1Round2, p1Round3, p1Round4, p1Round5;
    public string[] p2Round1, p2Round2, p2Round3, p2Round4, p2Round5;

    void Start()
    {
        humanScore = 0;
        catScore = 0;

        // 记录每个物体初始位置
        objectStartPositions = new Vector3[objectTransforms.Length];
        for (int i = 0; i < objectTransforms.Length; i++)
        {
            if (objectTransforms[i] != null)
            {
                objectStartPositions[i] = objectTransforms[i].position;
                objectTransforms[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        for (int round = 1; round <= totalRounds; round++)
        {
            // 关闭所有物体
            foreach (var obj in objectTransforms)
            {
                if (obj != null) obj.gameObject.SetActive(false);
            }

            // 设置当前轮物体
            if (round - 1 < objectTransforms.Length && objectTransforms[round - 1] != null)
            {
                currentObject = objectTransforms[round - 1];
                currentObject.gameObject.SetActive(true);

                currentObject.position = objectStartPositions[round - 1];
                currentObject.localScale = (Vector3.one)*0.6f;
            }

            yield return StartCoroutine(PlayRound(round));
        }

        EndGame();
    }

    IEnumerator PlayRound(int round)
    {
        ended = false;

        player1UI.SetRound(round);
        player2UI.SetRound(round);

        while (true)
        {
            string[][] p1Pools = { p1Round1, p1Round2, p1Round3, p1Round4, p1Round5 };
            string[][] p2Pools = { p2Round1, p2Round2, p2Round3, p2Round4, p2Round5 };

            string[] seq1 = GenerateSequence(round, p1Pools);
            string[] seq2 = GenerateSequence(round, p2Pools);

            KeyCode[] keySeq1 = ConvertToKeyCodes(seq1);
            KeyCode[] keySeq2 = ConvertToKeyCodes(seq2);

            player1.SetSequence(keySeq1);
            player2.SetSequence(keySeq2);

            player1UI.SetSequenceTexts(seq1);
            player2UI.SetSequenceTexts(seq2);

            while (true)
            {
                if (player1.targetSequence == null || player2.targetSequence == null)
                {
                    yield return null;
                    continue;
                }

                // 拉扯位置
                int diff = player1.currentIndex - player2.currentIndex;
                float t = Mathf.Clamp01((diff + round) / (2f * round));

                if (currentObject != null)
                {
                    currentObject.position = Vector3.Lerp(humanSidePos.position, catSidePos.position, t);
                }

                int result = Resolve(player1, player2);

                //  猫赢
                if (result == 1)
                {
                    catScore++;

                    if (catAnimator != null)
                        catAnimator.SetTrigger("CatGrab");

                    if (currentObject != null)
                        yield return StartCoroutine(SpringFly(currentObject));

                    yield break;
                }
                // 人赢
                else if (result == 2)
                {
                    humanScore++;

                    if (humanAnimator != null)
                        humanAnimator.SetTrigger("HumanGrab");

                    if (currentObject != null)
                        yield return StartCoroutine(MoveToHuman(currentObject));

                    yield break;
                }
                // 平局
                else if (result == 0)
                {
                    yield return new WaitForSeconds(1f);
                    break;
                }

                yield return null;
            }
        }
    }

    // 人赢：回到人侧
    IEnumerator MoveToHuman(Transform obj)
    {
        Vector3 start = obj.position;
        Vector3 target = humanSidePos.position;

        float t = 0;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            obj.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }

        obj.position = target;
    }

    // 猫赢：弹飞 + 弹性 + 旋转
    IEnumerator SpringFly(Transform obj)
    {
        Vector3 velocity = new Vector3(Random.Range(-2f, 2f), 5f, 0);
        float gravity = -9f;

        float time = 0;

        while (time < 1.2f)
        {
            time += Time.deltaTime;

            velocity.y += gravity * Time.deltaTime;
            obj.position += velocity * Time.deltaTime;

            // 弹性缩放
            float scale = 1f + Mathf.Sin(time * 20f) * 0.2f;
            obj.localScale = Vector3.one * scale;

            // 旋转
            obj.Rotate(Vector3.forward * 300f * Time.deltaTime);

            yield return null;
        }

        obj.gameObject.SetActive(false);
    }

    void EndGame()
    {
        if (ended) return;
        ended = true;

        if (humanScore > catScore)
        {
            Debug.Log("人赢");
            winLogic.Player1Win();
        }
        else if (catScore > humanScore)
        {
            Debug.Log("猫赢");
            winLogic.Player2Win();
        }
        else
        {
            Debug.Log("平局");
            SceneManager.LoadScene("ResultBridgeScene");
        }
    }

    

    void GoNext()
    {
        SceneManager.LoadScene("ResultBridgeScene");
    }

    public int Resolve(PlayerInput p1, PlayerInput p2)
    {
        if (p1.finished && !p2.finished) return 1;
        if (!p1.finished && p2.finished) return 2;
        if (p1.finished && p2.finished) return 0;
        return -1;
    }

    string[] GenerateSequence(int length, string[][] pools)
    {
        string[] seq = new string[length];
        for (int i = 0; i < length; i++)
        {
            string[] pool = pools[i];
            seq[i] = pool[Random.Range(0, pool.Length)];
        }
        return seq;
    }

    KeyCode[] ConvertToKeyCodes(string[] seq)
    {
        KeyCode[] keyCodes = new KeyCode[seq.Length];
        for (int i = 0; i < seq.Length; i++)
        {
            keyCodes[i] = CharToKeyCode(seq[i][0]);
        }
        return keyCodes;
    }

    KeyCode CharToKeyCode(char c)
    {
        char upper = char.ToUpper(c);

        if (upper >= 'A' && upper <= 'Z')
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), upper.ToString());

        if (char.IsDigit(c))
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + c);

        return KeyCode.None;
    }
}