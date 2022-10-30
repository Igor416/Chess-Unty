using System.Collections.Generic;
using UnityEngine;

class Pawn : Figure
{
    public readonly int direction;
    public Pawn(char x, char y, string color, int id, GameObject obj, Desk desk) : base(x, y, color, obj, desk)
    {
        name = color[0] + "p" + id;

        if (color == "white")
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
    }

    public override List<Cell> SpawnMoves()
    {
        List<Cell> moves = new List<Cell>();
        Cell left, up, forward, right;

        left = pos.RightUp(-1, direction);
        up = pos.Up(direction);
        right = pos.RightUp(1, direction);

        if (pos.yIndex == 1 || pos.yIndex == 6)
        {
            forward = pos.Up(direction * 2);
        }
        else
        {
            forward = new Cell();
        }
        if (!desk.ThereIsAFigure(left) || desk.GetFigureBy(left).color == color)
        {
            left = new Cell();
        }
        if (!desk.ThereIsAFigure(right) || desk.GetFigureBy(right).color == color)
        {
            right = new Cell();
        }
        if (desk.ThereIsAFigure(up) || desk.ThereIsAFigure(forward))
        {
            forward = new Cell();
        }
        if (desk.ThereIsAFigure(up))
        {
            up = new Cell();
        }
        moves.Add(left);
        moves.Add(right);
        moves.Add(forward);
        moves.Add(up);
        return moves;
    }

    public override bool CanMove(Cell where)
    {
        Cell left, up, forward, right;

        left = pos.RightUp(-1, direction);
        up = pos.Up(direction);
        right = pos.RightUp(1, direction);

        if (pos.yIndex == 1 || pos.yIndex == 6)
        {
            forward = pos.Up(direction * 2);
        }
        else
        {
            forward = new Cell();
        }

        if (where == left || where == right)
        {
            return desk.ThereIsAFigure(where) && desk.GetFigureBy(where).color != color;
        }
        else if (where == up || where == forward)
        {
            return !desk.ThereIsAFigure(where);
        }
        return false;
    }
}