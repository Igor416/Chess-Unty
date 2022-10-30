using System;
using System.Collections.Generic;
using UnityEngine;

class Bishop : Figure
{
    public Bishop(char x, char y, string color, int id, GameObject obj, Desk desk) : base(x, y, color, obj, desk)
    {
        name = color[0] + "b" + id;
    }

    public override List<Cell> SpawnMoves()
    {
        List<Cell> moves = new List<Cell>();
        moves.AddRange(CheckLine(1, 1));
        moves.AddRange(CheckLine(-1, 1));
        moves.AddRange(CheckLine(1, -1));
        moves.AddRange(CheckLine(-1, -1));
        return moves;
    }

    public override bool CanMove(Cell where)
    {
        if ((where - pos) % 11 == 0 && color != GetColor(where))
        {
            return CheckPos(where, Math.Sign(where.xIndex - pos.xIndex), Math.Sign(where.yIndex - pos.yIndex));
        }
        return false;
    }
}