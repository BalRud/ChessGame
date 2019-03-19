using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] Chessmans)
    {
        bool[,] allowedMoves = new bool[8, 8];

        Chessman c;
        int x;

        //Right
        x = CurrentX;
        while (true)
        {
            x++;
            if (x >= 8)
                break;

            c = Chessmans[x, CurrentY];

            if (c == null)
                allowedMoves[x, CurrentY] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[x, CurrentY] = true;

                break;
            }

        }

        //Left
        x = CurrentX;
        while (true)
        {
            x--;
            if (x < 0)
                break;

            c = Chessmans[x, CurrentY];
            if (c == null)
                allowedMoves[x, CurrentY] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[x, CurrentY] = true;

                break;
            }

        }

        //Up
        x = CurrentY;
        while (true)
        {
            x++;
            if (x >= 8)
                break;

            c = Chessmans[CurrentX, x];
            if (c == null)
                allowedMoves[CurrentX, x] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[CurrentX, x] = true;

                break;
            }

        }

        //Down
        x = CurrentY;
        while (true)
        {
            x--;
            if (x < 0)
                break;

            c = Chessmans[CurrentX, x];
            if (c == null)
                allowedMoves[CurrentX, x] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[CurrentX, x] = true;

                break;
            }

        }

        return allowedMoves;
    }
}
