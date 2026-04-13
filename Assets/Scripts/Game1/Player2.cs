using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Tile currentTile;
    public float moveDuration = 0.15f;

    private bool isMoving;
    private bool isBusy;

    [Header("建造")]
    public GameObject prefab;
    public float buildTime = 1.5f;

    [Header("动画")]
    public Animator animator;

    [Header("音效")]
    public AudioSource audioSource;

    public AudioClip moveClip;
    public AudioClip buildClip;

    void Update()
    {
        if (isMoving || isBusy || currentTile == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(currentTile.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(currentTile.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(currentTile.up);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TryDown();

        if (Input.GetKeyDown(KeyCode.Return))
            TryBuild();
    }


    void TryDown()
    {
        if (currentTile == null) return;

        // ?? 二楼直接下到一楼同列
        if (currentTile.floor == 1)
        {
            Tile target = GridManager.Instance.GetTile(0, currentTile.x);

            if (target != null)
            {
                StartCoroutine(MoveToTile(target));
                return;
            }
        }

        // ?? 一楼正常走楼梯
        if (currentTile.down != null)
        {
            StartCoroutine(MoveToTile(currentTile.down));
        }
    }
    // ================= MOVE =================

    void TryMove(Tile target)
    {
        if (target == null) return;
        StartCoroutine(MoveToTile(target));
    }

    IEnumerator MoveToTile(Tile target)
    {
        isMoving = true;

        float dx = target.transform.position.x - transform.position.x;

        if (dx > 0.01f)
            transform.localScale = new Vector3(0.22f, 0.22f, 1);
        else if (dx < -0.01f)
            transform.localScale = new Vector3(-0.22f, 0.22f, 1);

        if (animator != null)
            animator.SetTrigger("Move");

        if (audioSource != null && moveClip != null)
            audioSource.PlayOneShot(moveClip);

        Vector3 start = transform.position;
        Vector3 end = target.GetPosition(PlayerOwner.Player2);

        currentTile = target;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }

    // ================= BUILD =================

    void TryBuild()
    {
        if (currentTile == null) return;
        if (currentTile.currentObject != null) return;

        StartCoroutine(BuildRoutine());
    }

    IEnumerator BuildRoutine()
    {
        isBusy = true;

        if (animator != null)
            animator.SetTrigger("Build");

        if (audioSource != null && buildClip != null)
            audioSource.PlayOneShot(buildClip);

        yield return new WaitForSeconds(buildTime);

        GameObject obj = Instantiate(prefab, currentTile.centerPoint.position, Quaternion.identity);
        currentTile.currentObject = obj;

        isBusy = false;
    }
}
