using System.Collections.Generic;
using UnityEngine;

public abstract class Figure
{
    public Cell pos;
    public readonly string color;
    public string name;
    public GameObject obj;
    protected readonly Desk desk;

    public Figure(char x, char y, string color, GameObject obj, Desk desk)
    {
        pos = new Cell(x, y);
        this.color = color;
        this.obj = obj;
        this.obj.transform.position = new Vector2(pos.xPixel, pos.yPixel);
        this.desk = desk;
    }

    public void Move(Cell where, bool visualPart = true)
    {
        pos = where;
        if (visualPart) obj.transform.position = new Vector2(pos.xPixel, pos.yPixel);
    }

    public void Beat(Figure enemy, bool visualPart = true)
    {
        pos = enemy.pos;
        enemy.pos = new Cell();
        if (visualPart) obj.transform.position = new Vector2(pos.xPixel, pos.yPixel);
        if (visualPart) enemy.obj.transform.position = new Vector2(100f, 100f);
    }

    public abstract List<Cell> SpawnMoves();
    public abstract bool CanMove(Cell where);

    protected List<Cell> CheckLine(int x, int y)
    {
        List<Cell> moves = new List<Cell>();
        Cell middle = pos.RightUp(x, y);
        while (middle != new Cell())
        {
            if (desk.ThereIsAFigure(middle))
            {
                if (color == GetColor(middle))
                {
                    return moves;
                }
                moves.Add(middle);
                return moves;
            }
            moves.Add(middle);
            middle = middle.RightUp(x, y);
        }
        return moves;
    }

    protected bool CheckPos(Cell where, int x, int y)
    {
        Cell middle = pos.RightUp(x, y);
        while (middle != where)
        {
            if (desk.ThereIsAFigure(middle))
            {
                return false;
            }
            middle = middle.RightUp(x, y);
        }
        return true;
    }

    protected string GetColor(Cell where)
    {
        if (desk.GetFigureBy(where) is null)
        {
            return null;
        }
        return desk.GetFigureBy(where).color;
    }
}
