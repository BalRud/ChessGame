using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] Chessmans)
    {
        bool[,] allowedMoves = new bool[8, 8];

        Chessman c;
        int x, y;

        //Top Left
        x = CurrentX;
        y = CurrentY;
        while (true)
        {
            x--;
            y++;
            if (x < 0 || y >= 8)
                break;

            c = Chessmans[x, y];
            if (c == null)
                allowedMoves[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    allowedMoves[x, y] = true;

                break;
            }
        }

        //Top Right
        x = CurrentX;
        y = CurrentY;
        while (true)
        {
            x++;
            y++;
            if (x >= 8 || y >= 8)
                break;

            c = Chessmans[x, y];
            if (c == null)
                allowedMoves[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    allowedMoves[x, y] = true;

                break;
            }
        }

        //Down Left
        x = CurrentX;
        y = CurrentY;
        while (true)
        {
            x--;
            y--;
            if (x < 0 || y < 0)
                break;

            c = Chessmans[x, y];
            if (c == null)
                allowedMoves[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    allowedMoves[x, y] = true;

                break;
            }
        }

        //Down Right
        x = CurrentX;
        y = CurrentY;
        while (true)
        {
            x++;
            y--;
            if (x >= 8 || y < 0)
                break;

            c = Chessmans[x, y];
            if (c == null)
                allowedMoves[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    allowedMoves[x, y] = true;

                break;
            }
        }

        return allowedMoves;
    }
}
