using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    public LegPosition currentChoice;
    public bool hasChosen = false;

    void Update()
    {
        if (hasChosen) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Choose(LegPosition.TopLeft);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Choose(LegPosition.TopRight);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Choose(LegPosition.BottomLeft);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Choose(LegPosition.BottomRight);
    }

    void Choose(LegPosition pos)
    {
        currentChoice = pos;
        hasChosen = true;
        Debug.Log("Mouse chose: " + pos);
    }

    public void ResetChoice()
    {
        hasChosen = false;
    }
}