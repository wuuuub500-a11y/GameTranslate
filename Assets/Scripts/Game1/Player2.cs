using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Tile currentTile;
    public float moveDuration = 0.15f;

    private bool isMoving;


    [Header("生成设置")]
    public GameObject prefab;
    public float buildTime = 1.5f;

    [Header("动画")]
    public Animator animator;

    private bool isBusy;

    void Update()
    {

        if (isBusy) return;
        if (isMoving || currentTile == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(currentTile.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(currentTile.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(currentTile.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryDown();

       

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("准备拉屎");
            TryBuild();
        }
    }

    void TryDown()
    {
        //在二楼 → 直接到一楼同一x格子
        if (currentTile.floor == 1)
        {
            Tile target = GridManager.Instance.GetTile(0, currentTile.x);

            if (target != null)
            {
                StartCoroutine(MoveToTile(target));
                return;
            }
        }

        // 普通楼梯
        if (currentTile.down != null)
        {
            StartCoroutine(MoveToTile(currentTile.down));
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

    void TryBuild()
    {
        if (currentTile == null) return;

        // 已经有物体
        if (currentTile.currentObject != null) return;

        StartCoroutine(BuildRoutine());
    }

    IEnumerator BuildRoutine()
    {
        isBusy = true;

        // 播放动画
        if (animator != null)
            animator.SetTrigger("Build");

        yield return new WaitForSeconds(buildTime);


        Debug.Log("拉屎");
        // 生成
        GameObject obj = Instantiate(prefab, currentTile.centerPoint.position, Quaternion.identity);
        currentTile.currentObject = obj;

        isBusy = false;
    }
}
