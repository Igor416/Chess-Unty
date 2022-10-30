using System.Collections.Generic;
using System;
using UnityEngine;

public class Cell
{
    public readonly short xIndex, yIndex;
    public readonly string name;
    public float xPixel, yPixel;
    private readonly char x, y;
    private const string letters = "abcdefgh";
    private const string numbers = "12345678";

    public Cell()
    {
        xIndex = -1;
        yIndex = -1;
        name = null;
        xPixel = 100f;
        yPixel = 100f;
    }

    public Cell(string name)
    {
        x = name[0];
        y = name[1];
        this.name = name;
        try
        {
            xIndex = (short)letters.IndexOf(x);
            yIndex = (short)numbers.IndexOf(y);
            xPixel = -3.16f + xIndex * 0.9f;
            yPixel = -3.16f + yIndex * 0.9f;
        }
        catch (IndexOutOfRangeException)
        {
            xIndex = -1;
            yIndex = -1;
            this.name = null;
            xPixel = 100f;
            yPixel = 100f;
        }
    }

    public Cell(int index)
    {
        yIndex = (short)Math.DivRem(index, 8, out int remainder);
        xIndex = (short)remainder;
        try
        {
            x = letters[xIndex];
            y = numbers[yIndex];
            name = Convert.ToString(x) + Convert.ToString(y);
            xPixel = -3.16f + xIndex * 0.9f;
            yPixel = -3.16f + yIndex * 0.9f;
        }
        catch (IndexOutOfRangeException)
        {
            xIndex = -1;
            yIndex = -1;
            name = null;
            xPixel = 100f;
            yPixel = 100f;
        }
    }

    public Cell(char x, char y)
    {
        this.x = x;
        this.y = y;
        name = Convert.ToString(x) + Convert.ToString(y);
        try
        {
            xIndex = (short)letters.IndexOf(x);
            yIndex = (short)numbers.IndexOf(y);
            xPixel = -3.16f + xIndex * 0.9f;
            yPixel = -3.16f + yIndex * 0.9f;
        }
        catch (IndexOutOfRangeException)
        {
            xIndex = -1;
            yIndex = -1;
            name = null;
            xPixel = 100f;
            yPixel = 100f;
        }
    }

    public Cell(float xPixel, float yPixel)
    {
        xIndex = (short)Math.Round((xPixel + 3.16f) / 0.9f);
        yIndex = (short)Math.Round((yPixel + 3.16f) / 0.9f);
        this.xPixel = -3.16f + xIndex * 0.9f;
        this.yPixel = -3.16f + yIndex * 0.9f;
        try
        {
            x = letters[xIndex];
            y = numbers[yIndex];
            name = Convert.ToString(x) + Convert.ToString(y);
        }
        catch (IndexOutOfRangeException)
        {
            xIndex = -1;
            yIndex = -1;
            name = null;
            this.xPixel = 100f;
            this.yPixel = 100f;
        }
    }

    public static int operator -(Cell cell1, Cell cell2)
    {
        return Math.Abs(cell1.xIndex - cell2.xIndex) * 10 + Math.Abs(cell1.yIndex - cell2.yIndex);
    }

    public static bool operator ==(Cell cell1, Cell cell2)
    {
        if (cell1 is null || cell2 is null)
        {
            return false;
        }
        return cell1.name == cell2.name;
    }

    public static bool operator !=(Cell cell1, Cell cell2)
    {
        if (cell1 is null || cell2 is null)
        {
            return false;
        }
        return cell1.name != cell2.name;
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }

        var second = o as Cell;

        return second != null && name == second.name;
    }

    public override int GetHashCode()
    {
        return xIndex * 10 + yIndex;
    }

    public Cell Right(int value)
    {
        try
        {
            return new Cell(letters[xIndex + value], y);
        }
        catch (IndexOutOfRangeException)
        {
            return new Cell();
        }
    }

    public Cell Up(int value)
    {
        try
        {
            return new Cell(x, numbers[yIndex + value]);
        }
        catch (IndexOutOfRangeException)
        {
            return new Cell();
        }
    }

    public Cell RightUp(int xValue, int yValue)
    {
        try
        {
            return new Cell(letters[xIndex + xValue], numbers[yIndex + yValue]);
        }
        catch (IndexOutOfRangeException)
        {
            return new Cell();
        }
    }
}
