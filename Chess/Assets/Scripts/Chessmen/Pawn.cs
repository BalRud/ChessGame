using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] Chessmans)
    {
        bool[,] allowedMoves = new bool[8, 8];
        Chessman c, c2;
        int[] e = BoardManager.Instance.EnPassantMove;

        //White team
        if (isWhite)
        {
            //Diagonal Left
            if (CurrentX != 0 && CurrentY != 7)
            {
                if (e[0] == CurrentX - 1 && e[1] == CurrentY + 1)
                    allowedMoves[CurrentX - 1, CurrentY + 1] = true;

                c = Chessmans[CurrentX - 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                    allowedMoves[CurrentX - 1, CurrentY + 1] = true;
            }
            //Diagonal Right
            if (CurrentX != 7 && CurrentY != 7)
            {
                if (e[0] == CurrentX + 1 && e[1] == CurrentY + 1)
                    allowedMoves[CurrentX + 1, CurrentY + 1] = true;

                c = Chessmans[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                    allowedMoves[CurrentX + 1, CurrentY + 1] = true;
            }

            //Middle
            if (CurrentY != 7)
            {
                c = Chessmans[CurrentX, CurrentY + 1];
                if (c == null)
                    allowedMoves[CurrentX, CurrentY + 1] = true;
            }

            //Midle on first move
            if (CurrentY == 1)
            {
                c = Chessmans[CurrentX, CurrentY + 1];
                c2 = Chessmans[CurrentX, CurrentY + 2];
                if (c == null && c2 == null)
                    allowedMoves[CurrentX, CurrentY + 2] = true;

            }
        }

        //Black team
        else
        {
            //Diagonal Left
            if (CurrentX != 0 && CurrentY != 0)
            {
                if (e[0] == CurrentX - 1 && e[1] == CurrentY - 1)
                    allowedMoves[CurrentX - 1, CurrentY - 1] = true;

                c = Chessmans[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                    allowedMoves[CurrentX - 1, CurrentY - 1] = true;
            }
            //Diagonal Right
            if (CurrentX != 7 && CurrentY != 0)
            {
                if (e[0] == CurrentX + 1 && e[1] == CurrentY - 1)
                    allowedMoves[CurrentX + 1, CurrentY - 1] = true;

                c = Chessmans[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                    allowedMoves[CurrentX + 1, CurrentY - 1] = true;
            }

            //Middle
            if (CurrentY != 0)
            {
                c = Chessmans[CurrentX, CurrentY - 1];
                if (c == null)
                    allowedMoves[CurrentX, CurrentY - 1] = true;
            }

            //Midle on first move
            if (CurrentY == 6)
            {
                c = Chessmans[CurrentX, CurrentY - 1];
                c2 = Chessmans[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                    allowedMoves[CurrentX, CurrentY - 2] = true;

            }

        }

        return allowedMoves;
    }
}
