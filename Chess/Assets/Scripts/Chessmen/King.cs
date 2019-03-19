using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    public override bool[,] PossibleMove(Chessman[,] Chessmans)
    {
        bool[,] allowedMoves = new bool[8, 8];
        bool W = BoardManager.Instance.WhiteKingMoved;
        bool B = BoardManager.Instance.BlackKingMoved;
        bool H1 = BoardManager.Instance.RookH1Moved;
        bool A1 = BoardManager.Instance.RookA1Moved;
        bool H8 = BoardManager.Instance.RookH8Moved;
        bool A8 = BoardManager.Instance.RookA8Moved;

        Chessman c;
        int x, y;
        //Top Side
        x = CurrentX - 1;
        y = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (x >= 0 && x < 8)
                {
                    c = Chessmans[x, y];
                    if (c == null || isWhite != c.isWhite)
                    {
                        allowedMoves[x, y] = true;
                    }
                }
                x++;
            }
        }

        //Downn Side
        x = CurrentX - 1;
        y = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (x >= 0 && x < 8)
                {
                    c = Chessmans[x, y];
                    if (c == null)
                        allowedMoves[x, y] = true;
                    else if (isWhite != c.isWhite)
                        allowedMoves[x, y] = true;
                }
                x++;
            }
        }

        //Midle Left
        if (CurrentX != 0)
        {
            c = Chessmans[CurrentX - 1, CurrentY];
            if (c == null)
                allowedMoves[CurrentX - 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                allowedMoves[CurrentX - 1, CurrentY] = true;
        }

        //Midle Right
        if (CurrentX != 7)
        {
            c = Chessmans[CurrentX + 1, CurrentY];
            if (c == null)
                allowedMoves[CurrentX + 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                allowedMoves[CurrentX + 1, CurrentY] = true;
        }

        //Castling Move
        //White team
        if (BoardManager.Instance.isWhiteTurn)
        {
            x = CurrentX;
            y = CurrentY;
            if (!W && !H1 && y == 0)
            {
                if (BoardManager.Instance.Chessmans[x + 1, y] == null)
                    if (BoardManager.Instance.Chessmans[x + 2, y] == null)
                        if (PawnCheckShort(x, y + 1))
                        {
                            if (BoardManager.Instance.CastlingMoveShortCheck(x, y))
                                allowedMoves[CurrentX + 2, CurrentY] = true;
                        }
            }
            if (!W && !A1 && y == 0)
            {
                if (BoardManager.Instance.Chessmans[x - 1, y] == null)
                    if (BoardManager.Instance.Chessmans[x - 2, y] == null)
                        if (BoardManager.Instance.Chessmans[x - 3, y] == null)
                            if (PawnCheckLong(x, y + 1))
                            {
                                if (BoardManager.Instance.CastlingMoveLongCheck(x, y))
                                    allowedMoves[CurrentX + -2, CurrentY] = true;
                            }
            }
        }
        //Black team
        if (!BoardManager.Instance.isWhiteTurn)
        {
            x = CurrentX;
            y = CurrentY;
            if (!B && !H8 && y == 7)
            {
                if (BoardManager.Instance.Chessmans[x + 1, y] == null)
                    if (BoardManager.Instance.Chessmans[x + 2, y] == null)
                        if (PawnCheckShort(x, y - 1))
                        {
                            if (BoardManager.Instance.CastlingMoveShortCheck(x, y))
                                allowedMoves[CurrentX + 2, CurrentY] = true;
                        }
            }
            if (!B && !A8 && y == 7)
            {
                if (BoardManager.Instance.Chessmans[x - 1, y] == null)
                    if (BoardManager.Instance.Chessmans[x - 2, y] == null)
                        if (BoardManager.Instance.Chessmans[x - 3, y] == null)
                            if (PawnCheckLong(x, y - 1))
                            {
                                if (BoardManager.Instance.CastlingMoveLongCheck(x, y))
                                    allowedMoves[CurrentX + -2, CurrentY] = true;
                            }
            }
        }
        return allowedMoves;
    }

    public bool PawnCheckShort(int x, int y)
    {
        Chessman c;
        for (int i = 0; i < 4; i++)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c != null && c.GetType() == typeof(Pawn) && c.isWhite != BoardManager.Instance.isWhiteTurn)
            {
                return false;
            }
            x++;
        }
        return true;
    }
    public bool PawnCheckLong(int x, int y)
    {
        Chessman c;
        for (int i = 0; i < 5; i++)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c != null && c.GetType() == typeof(Pawn) && c.isWhite != BoardManager.Instance.isWhiteTurn)
            {
                return false;
            }
            x--;
        }
        return true;
    }
}
