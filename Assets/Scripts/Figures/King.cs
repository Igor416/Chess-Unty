using System.Collections.Generic;
using UnityEngine;

class King : Figure
{
    public King(char x, char y, string color, GameObject obj, Desk desk) : base(x, y, color, obj, desk)
    {
        name = color[0] + "K";
    }

    public override List<Cell> SpawnMoves()
    {
        List<Cell> moves = new List<Cell>
        {
            pos.RightUp(1, 1),
            pos.RightUp(1, -1),
            pos.RightUp(-1, 1),
            pos.RightUp(-1, -1),
            pos.RightUp(1, 0),
            pos.RightUp(0, 1),
            pos.RightUp(-1, 0),
            pos.RightUp(0, -1)
        };
        for (int i = 0; i < moves.Count; i++)
        {
            if (color == GetColor(moves[i]))
            {
                moves[i] = new Cell();
            }
        }
        return moves;
    }

    public override bool CanMove(Cell where)
    {
        return (where - pos == 1 || where - pos == 10 || where - pos == 11) && color != GetColor(where);
    }
}