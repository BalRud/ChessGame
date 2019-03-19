using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promotion : MonoBehaviour
{

    int index;

    public void PressQueen()
    {
        if (!BoardManager.Instance.isWhiteTurn)
            index = 1;
        else if (BoardManager.Instance.isWhiteTurn)
            index = 7;
        PieceSwitch();
    }

    public void PressRook()
    {
        if (!BoardManager.Instance.isWhiteTurn)
            index = 2;
        else if (BoardManager.Instance.isWhiteTurn)
            index = 8;
        PieceSwitch();
    }

    public void PressBishop()
    {
        if (!BoardManager.Instance.isWhiteTurn)
            index = 3;
        else if (BoardManager.Instance.isWhiteTurn)
            index = 9;
        PieceSwitch();
    }

    public void PressKnight()
    {
        if (!BoardManager.Instance.isWhiteTurn)
            index = 4;
        else if (BoardManager.Instance.isWhiteTurn)
            index = 10;
        PieceSwitch();
    }

    public void PieceSwitch()
    {
        Chessman c;
        int y;
        if (!BoardManager.Instance.isWhiteTurn)
            y = 7;
        else
            y = 0;
        for (int x = 0; x < 8; x++)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c != null && c.GetType() == typeof(Pawn))
            {
                BoardManager.Instance.activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
                BoardManager.Instance.SpawnChessman(index, x, y);
                c = BoardManager.Instance.Chessmans[x, y];
            }
        }
    }

}
