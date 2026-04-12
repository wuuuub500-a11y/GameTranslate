using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [Header("生成物")]
    public GameObject currentObject;
    [Header("格子坐标")]
    public int floor; // 0 = 一楼，1 = 二楼
    public int x;

    public Tile left;
    public Tile right;
    public Tile up;
    public Tile down;

    [Header("站位偏移点")]
    public Transform centerPoint;
    public Transform leftSlot;
    public Transform rightSlot;

    public Vector3 GetPosition(PlayerOwner owner)
    {
        if (owner == PlayerOwner.Player1)
            return rightSlot ? rightSlot.position : centerPoint.position;

        if (owner == PlayerOwner.Player2)
            return leftSlot ? leftSlot.position : centerPoint.position;

        return centerPoint.position;
    }
}

public enum PlayerOwner
{
    Player1,
    Player2
}
