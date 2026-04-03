using UnityEngine;
public enum LegPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
public class PlayerHuman : MonoBehaviour
{
    public LegPosition currentChoice;
    public bool hasChosen = false;

    void Update()
    {
        if (hasChosen) return;

        if (Input.GetKeyDown(KeyCode.W))
            Choose(LegPosition.TopLeft);

        if (Input.GetKeyDown(KeyCode.D))
            Choose(LegPosition.TopRight);

        if (Input.GetKeyDown(KeyCode.A))
            Choose(LegPosition.BottomLeft);

        if (Input.GetKeyDown(KeyCode.S))
            Choose(LegPosition.BottomRight);
    }

    void Choose(LegPosition pos)
    {
        currentChoice = pos;
        hasChosen = true;
        Debug.Log("Human chose: " + pos);
    }

    public void ResetChoice()
    {
        hasChosen = false;
    }
}