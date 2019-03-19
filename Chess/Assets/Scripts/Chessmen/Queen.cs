using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] Chessmans)
    {

        bool[,] allowedMoves = new bool[8, 8];

        Chessman c;
        int x, y;

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
        y = CurrentY;
        while (true)
        {
            y++;
            if (y >= 8)
                break;

            c = Chessmans[CurrentX, y];
            if (c == null)
                allowedMoves[CurrentX, y] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[CurrentX, y] = true;

                break;
            }

        }

        //Down
        y = CurrentY;
        while (true)
        {
            y--;
            if (y < 0)
                break;

            c = Chessmans[CurrentX, y];
            if (c == null)
                allowedMoves[CurrentX, y] = true;
            else
            {
                if (c.isWhite != isWhite)
                    allowedMoves[CurrentX, y] = true;

                break;
            }

        }

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
