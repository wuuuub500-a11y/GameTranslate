using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public Tile currentTile;
    public float moveDuration = 0.15f;

    private bool isMoving;

    [Header("清理设置")]
    public float clearTime = 1.5f;

    [Header("动画")]
    public Animator animator;

    private bool isBusy;

    void Update()
    {
        if (isMoving || currentTile == null) return;

        if (isBusy) return;

        if (Input.GetKeyDown(KeyCode.A)) TryMove(currentTile.left);
        if (Input.GetKeyDown(KeyCode.D)) TryMove(currentTile.right);
        if (Input.GetKeyDown(KeyCode.W)) TryMove(currentTile.up);
        if (Input.GetKeyDown(KeyCode.S)) TryMove(currentTile.down);

        

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryClear();
        }
    }

    void TryMove(Tile target)
    {
        if (target == null) return;
        StartCoroutine(MoveToTile(target));
    }

    System.Collections.IEnumerator MoveToTile(Tile target)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = target.GetPosition(PlayerOwner.Player1);

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

    void TryClear()
    {
        if (currentTile == null) return;

        if (currentTile.currentObject == null) return;

        StartCoroutine(ClearRoutine());
    }

    IEnumerator ClearRoutine()
    {
        isBusy = true;

        // 播放动画
        if (animator != null)
            animator.SetTrigger("Clear");

        yield return new WaitForSeconds(clearTime);

        Destroy(currentTile.currentObject);
        currentTile.currentObject = null;

        isBusy = false;
    }
}
