using System.Collections.Generic;
using UnityEngine;

class Knight : Figure
{
    public Knight(char x, char y, string color, int id, GameObject obj, Desk desk) : base(x, y, color, obj, desk)
    {
        name = color[0] + "k" + id;
    }

    public override List<Cell> SpawnMoves()
    {
        List<Cell> moves = new List<Cell>
        {
            pos.RightUp(1, 2),
            pos.RightUp(2, 1),
            pos.RightUp(-1, 2),
            pos.RightUp(-2, 1),
            pos.RightUp(1, -2),
            pos.RightUp(2, -1),
            pos.RightUp(-1, -2),
            pos.RightUp(-2, -1)
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
        return (where - pos == 12 || where - pos == 21) && color != GetColor(where);
    }
}