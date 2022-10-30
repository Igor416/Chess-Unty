using System;
using System.Collections.Generic;
using UnityEngine;

class Queen : Figure
{
    public Queen(char x, char y, string color, GameObject obj, Desk desk, int id = 0) : base(x, y, color, obj, desk)
    {
        if (id == 0)
        {
            name = color[0] + "q";
        }
        else
        {
            name = color[0] + "q" + id;
        }
    }

    public override List<Cell> SpawnMoves()
    {
        List<Cell> moves = new List<Cell>();
        moves.AddRange(CheckLine(1, 1));
        moves.AddRange(CheckLine(-1, 1));
        moves.AddRange(CheckLine(1, -1));
        moves.AddRange(CheckLine(-1, -1));
        moves.AddRange(CheckLine(1, 0));
        moves.AddRange(CheckLine(-1, 0));
        moves.AddRange(CheckLine(0, 1));
        moves.AddRange(CheckLine(0, -1));
        return moves;
    }

    public override bool CanMove(Cell where)
    {
        if ((where - pos) % 11 == 0 && color != GetColor(where))
        {
            return CheckPos(where, Math.Sign(where.xIndex - pos.xIndex), Math.Sign(where.yIndex - pos.yIndex));
        }
        else if ((where - pos) % 10 == 0 && color != GetColor(where))
        {
            return CheckPos(where, Math.Sign(where.xIndex - pos.xIndex), 0);
        }
        else if ((where - pos) < 10 && color != GetColor(where))
        {
            return CheckPos(where, 0, Math.Sign(where.yIndex - pos.yIndex));
        }
        return false;
    }
}