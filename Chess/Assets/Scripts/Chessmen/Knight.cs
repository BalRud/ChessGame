using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] PosMChessmans)
    {
        bool[,] allowedMoves = new bool[8, 8];

        //UpLeft
        KnightMove(CurrentX - 1, CurrentY + 2, PosMChessmans, ref allowedMoves);

        //UpRight
        KnightMove(CurrentX + 1, CurrentY + 2, PosMChessmans, ref allowedMoves);

        //RightUP
        KnightMove(CurrentX + 2, CurrentY + 1, PosMChessmans, ref allowedMoves);

        //RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, PosMChessmans, ref allowedMoves);

        //RightUP
        KnightMove(CurrentX + 2, CurrentY + 1, PosMChessmans, ref allowedMoves);

        //DownLeft
        KnightMove(CurrentX - 1, CurrentY - 2, PosMChessmans, ref allowedMoves);

        //DownRight
        KnightMove(CurrentX + 1, CurrentY - 2, PosMChessmans, ref allowedMoves);

        //LeftUP
        KnightMove(CurrentX - 2, CurrentY + 1, PosMChessmans, ref allowedMoves);

        //LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, PosMChessmans, ref allowedMoves);

        return allowedMoves;
    }

    public void KnightMove(int x, int y, Chessman[,] PosMChessmans, ref bool[,] allowedMoves)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = PosMChessmans[x, y];
            if (c == null)
                allowedMoves[x, y] = true;
            else if (isWhite != c.isWhite)
                allowedMoves[x, y] = true;
        }
    }
}
