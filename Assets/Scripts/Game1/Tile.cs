using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile left;
    public Tile right;
    public Tile up;
    public Tile down;

    [Header("վλƫ�Ƶ�")]
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
