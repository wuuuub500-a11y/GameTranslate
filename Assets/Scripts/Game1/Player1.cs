using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public Tile currentTile;
    public float moveDuration = 0.15f;

    private bool isMoving;
    private bool isBusy;

    [Header("清理")]
    public float clearTime = 1.5f;

    [Header("动画")]
    public Animator animator;

    [Header("音效")]
    public AudioSource audioSource;

    public AudioClip moveClip;
    public AudioClip clearClip;

    void Update()
    {
        if (isMoving || isBusy || currentTile == null) return;

        if (Input.GetKeyDown(KeyCode.A)) TryMove(currentTile.left);
        if (Input.GetKeyDown(KeyCode.D)) TryMove(currentTile.right);
        if (Input.GetKeyDown(KeyCode.W)) TryMove(currentTile.up);
        if (Input.GetKeyDown(KeyCode.S)) TryMove(currentTile.down);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            TryClear();
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

    // ================= CLEAR =================

    void TryClear()
    {
        if (currentTile == null) return;
        if (currentTile.currentObject == null) return;

        StartCoroutine(ClearRoutine());
    }

    IEnumerator ClearRoutine()
    {
        isBusy = true;

        if (animator != null)
            animator.SetTrigger("Clear");

        if (audioSource != null && clearClip != null)
            audioSource.PlayOneShot(clearClip);

        yield return new WaitForSeconds(clearTime);

        Destroy(currentTile.currentObject);
        currentTile.currentObject = null;

        isBusy = false;
    }
}
