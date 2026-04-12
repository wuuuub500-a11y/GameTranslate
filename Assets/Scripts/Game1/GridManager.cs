using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    private List<Tile> tiles = new List<Tile>();

    void Awake()
    {
        Instance = this;
        tiles.AddRange(FindObjectsOfType<Tile>());
    }

    public Tile GetTile(int floor, int x)
    {
        return tiles.Find(t => t.floor == floor && t.x == x);
    }
}
