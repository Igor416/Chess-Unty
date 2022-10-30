using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Desk : MonoBehaviour
{
    //prefabs
    public GameObject greenDot, redDot;
    public GameObject check;

    //castlings: { rookId: isLegal }
    private readonly IDictionary<string, bool> cast = new Dictionary<string, bool>()
    {
        { "w1", true },
        { "w2", true },
        { "b1", true },
        { "b2", true }
    };


    //iterables
    private readonly List<GameObject> dots = new List<GameObject>();
    public Figure[] figures = new Figure[32];


    public void ShowMoves(Figure figure, string mode, int round)
    {
        DeleteDots();

        if (figure != null && figure.color == mode)
        {
            if (figure.name[1] == 'p' && round > 1 && EnPassant(figure as Pawn))
            {
                Cell move = new Cell();
                if (History.LastMove[3][1] == '4') move = new Cell(History.LastMove[3]).Up(-1);
                else if (History.LastMove[3][1] == '5') move = new Cell(History.LastMove[3]).Up(1);

                AddDot(redDot, move);
            }
            else if (figure.name[1] == 'K')
            {
                for (int i = 1; i < 3; i++)
                {
                    string index = i.ToString();
                    if (cast[$"{mode[0]}{index}"])
                    {
                        char xAxis, yAxis;
                        switch (index)
                        {
                            case "1":
                                xAxis = 'a';
                                break;
                            case "2":
                                xAxis = 'h';
                                break;
                            default:
                                return;
                        }
                        switch (mode)
                        {
                            case "white":
                                yAxis = '1';
                                break;
                            case "black":
                                yAxis = '8';
                                break;
                            default:
                                return;
                        }

                        King king = GetFigureBy(mode[0] + "K") as King;
                        Rook rook = GetFigureBy(mode[0] + "r" + index) as Rook;
                        if (king is King || rook is Rook)
                        {
                            break;
                        }
                        if (king.pos == new Cell('e', yAxis) && rook.pos == new Cell(xAxis, yAxis))
                        {
                            bool error = false;
                            int direction = Math.Sign(rook.pos.xIndex - king.pos.xIndex);

                            Cell kingPos = king.pos;
                            Cell middle = king.pos.Right(direction);
                            while (middle != rook.pos.Right(-direction))
                            {
                                king.pos = middle;
                                if (ThereIsAFigure(middle.Right(direction)))
                                {
                                    error = true;
                                    break;
                                }
                                else if (IsCheck())
                                {
                                    error = true;
                                    break;
                                }
                                middle = middle.Right(direction);
                            }
                            king.pos = kingPos;
                            if (!error)
                            {
                                AddDot(greenDot, rook.pos);
                            }
                        }
                    }
                }
            }
            foreach (Cell move in figure.SpawnMoves())
            {
                if (move != new Cell())
                {
                    if (ThereIsAFigure(move))
                    {
                        Figure enemy = GetFigureBy(move);
                        History.SetLastMove(figure.name, figure.pos.name, enemy.name, enemy.pos.name);
                        figure.Beat(enemy, visualPart: false);
                    }
                    else
                    {
                        History.SetLastMove(figure.name, figure.pos.name, "", move.name);
                        figure.Move(move, visualPart: false);
                    }
                    if (mode == "white")
                    {
                        mode = "black";
                    }
                    else
                    {
                        mode = "white";
                    }
                    if (IsCheck() && WhatCheck() == mode)
                    {
                        MoveBack(visualPart: false);
                        History.MoveBack();
                        if (mode == "white")
                        {
                            mode = "black";
                        }
                        else
                        {
                            mode = "white";
                        }
                        continue;
                    }
                    else
                    {
                        MoveBack(visualPart: false);
                        History.MoveBack();
                        if (mode == "white")
                        {
                            mode = "black";
                        }
                        else
                        {
                            mode = "white";
                        }

                        if (ThereIsAFigure(move))
                        {
                            AddDot(redDot, move);
                        }
                        else
                        {
                            AddDot(greenDot, move);
                        }
                    }
                }
            }
        }
    }

    public void Move(Cell cell, Figure figure, string mode, int round)
    {
        switch (figure.name)
        {
            case "wr1":
                cast["w1"] = false;
                break;
            case "wr2":
                cast["w2"] = false;
                break;
            case "br1":
                cast["b1"] = false;
                break;
            case "br2":
                cast["b2"] = false;
                break;
            case "wK":
                cast["w1"] = false;
                cast["w2"] = false;
                break;
            case "bK":
                cast["b1"] = false;
                cast["b2"] = false;
                break;
        }

        if (ThereIsAFigure(cell))
        {
            Figure enemy = GetFigureBy(cell);
            if (enemy.color != mode)
            {
                History.SetLastMove(figure.name, figure.pos.name, enemy.name, cell.name);
                figure.Beat(enemy);
            }
            else
            {
                Figure rook = enemy;
                Figure king = figure as King;
                int direction = Math.Sign(rook.pos.xIndex - king.pos.xIndex);
                History.SetLastMove(king.name, king.pos.name, rook.name, rook.pos.name);
                rook.Move(new Cell(king.pos.Right(direction).name));
                king.Move(new Cell(king.pos.Right(direction * 2).name));
            }
        }
        else
        {
            if (figure.name[1] == 'p' && round > 1 && EnPassant(figure as Pawn))
            {
                Cell move = new Cell();
                if (History.LastMove[3][1] == '4') move = new Cell(History.LastMove[3]).Up(-1);
                else if (History.LastMove[3][1] == '5') move = new Cell(History.LastMove[3]).Up(1);

                Figure enemy = GetFigureBy(new Cell(History.LastMove[3]));
                History.SetLastMove(figure.name, figure.pos.name, enemy.name, enemy.pos.name);
                figure.Beat(enemy);
                figure.Move(move);
                figure.obj.transform.position = new Vector2(move.xPixel, move.yPixel);
            }
            else
            {
                History.SetLastMove(figure.name, figure.pos.name, "", cell.name);
                figure.Move(cell);
            }
        }

        DeleteDots();
    }

    

    public void MoveBack(bool visualPart)
    {
        if (visualPart == false)
        {
            Figure figure1 = GetFigureBy(History.LastMove[0]);
            Cell cell1 = new Cell(History.LastMove[1]);
            Figure figure2 = GetFigureBy(History.LastMove[2]);
            Cell cell2 = new Cell(History.LastMove[3]);
            figure1.Move(cell1, visualPart);

            if (figure2 != null)
            {
                figure2.Move(cell2, visualPart);
            }
            return;
        }
    } //for spawning dots

    //functions for Figure class
    public Figure GetFigureBy(string name)
    {
        foreach (Figure figure in figures)
        {
            if (figure != null && figure.name == name)
            {
                return figure;
            }
        }
        return null;
    }

    public Figure GetFigureBy(Cell cell)
    {
        foreach (Figure figure in figures)
        {
            if (figure != null && figure.pos == cell)
            {
                
                return figure;
            }
        }
        return null;
    }

    public bool ThereIsAFigure(Cell cell)
    {
        if (cell != new Cell())
        {
            return GetFigureBy(cell) != null;
        }
        return false;
    }

    public bool ThereIsADot(Cell cell)
    {
        Vector2 pos;
        foreach (GameObject dot in dots)
        {
            pos = dot.transform.position;
            if (new Cell(pos.x, pos.y) == cell)
            {
                return true;
            }
        }
        return false;
    } //is similar, but not used in Figure class

    //functions for checking events
    public bool IsCheck()
    {
        King whiteKing = figures[15] as King;
        for (int i = figures.Length / 2; i < figures.Length; i++)
        {
            if (figures[i] != null && figures[i].CanMove(whiteKing.pos))
            {
                return true;
            }
        }

        King blackKing = figures[31] as King;
        for (int i = 0; i < figures.Length / 2; i++)
        {
            if (figures[i] != null && figures[i].CanMove(blackKing.pos))
            {
                return true;
            }
        }
        return false;
    }

    public string WhatCheck()
    {
        King whiteKing = figures[15] as King;
        for (int i = figures.Length / 2; i < figures.Length; i++)
        {
            if (figures[i] != null && figures[i].CanMove(whiteKing.pos))
            {
                return "black";
            }
        }

        King blackKing = figures[31] as King;
        for (int i = 0; i < figures.Length / 2; i++)
        {
            if (figures[i] != null && figures[i].CanMove(blackKing.pos))
            {
                return "white";
            }
        }
        return null;
    }

    public bool IsMate(string mode)
    {
        int i;
        int length;
        if (mode == "white")
        {
            i = 0;
            length = figures.Length / 2;
        }
        else
        {
            i = figures.Length / 2 - 1;
            length = figures.Length;
        }

        for (; i < length; i++)
        {
            if (figures[i] != null && figures[i].pos != new Cell())
            {
                foreach (Cell move in figures[i].SpawnMoves())
                {
                    if (move != new Cell())
                    {
                        if (ThereIsAFigure(move))
                        {
                            Figure enemy = GetFigureBy(move);
                            History.SetLastMove(figures[i].name, figures[i].pos.name, enemy.name, enemy.pos.name);
                            figures[i].Beat(enemy, visualPart: false);
                        }
                        else
                        {
                            History.SetLastMove(figures[i].name, figures[i].pos.name, "", move.name);
                            figures[i].Move(move, visualPart: false);
                        }
                        if (mode == "white")
                        {
                            mode = "black";
                        }
                        else
                        {
                            mode = "white";
                        }
                        if (IsCheck() && WhatCheck() == mode)
                        {
                            MoveBack(visualPart: false);
                            History.MoveBack();
                            if (mode == "white")
                            {
                                mode = "black";
                            }
                            else
                            {
                                mode = "white";
                            }
                        }
                        else
                        {
                            MoveBack(visualPart: false);
                            History.MoveBack();
                            if (mode == "white")
                            {
                                mode = "black";
                            }
                            else
                            {
                                mode = "white";
                            }

                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //methods for shortening code
    

    private void AddDot(GameObject prefab, Cell cell)
    {
        GameObject dot = prefab;
        dot.transform.position = new Vector2(cell.xPixel, cell.yPixel);
        dot = Instantiate(dot, dot.transform.position, Quaternion.identity);
        dots.Add(dot);
    }

    public void ShowCheck(string status)
    {
        GameObject text = Instantiate(check, check.transform.position, Quaternion.identity);
        text.GetComponent<TextMesh>().text = status;
    }

    public void DeleteDots()
    {
        foreach (GameObject dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();
    }

    public void DeleteFigures()
    {
        foreach (Figure figure in figures)
        {
            if (figure != null) Destroy(figure.obj);
        }
    }

    public Figure[] CreateFigures(string figurePositions, GameObject[] figurePrefabs)
    {
        Figure[] figures = new Figure[32];
        Dictionary<string, int> ids = new Dictionary<string, int>()
        {
            { "wknight", 3 },
            { "wbishop", 3 },
            { "wrook", 3 },
            { "wqueen", 1 },
            { "bknight", 3 },
            { "bbishop", 3 },
            { "brook", 3 },
            { "bqueen", 1 }
        };
        if (figurePositions is null)
        {
            figurePositions = "wrwkwbwqwKwbwkwrwpwpwpwpwpwpwpwpNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNbpbpbpbpbpbpbpbpbrbkbbbqbKbbbkbr";
        }

        string[] promoted = new string[8] { "wk", "wb", "wr", "wq", "bk", "bb", "br", "bq" };
        string mode, pos;
        int index, term1, term2;
        for (int i = 0; i < promoted.Length; i++)
        {
            if (figurePositions[i] == 'w')
            {
                mode = "white";
                term1 = 0;
                term2 = 0;
            }
            else
            {
                mode = "black";
                term1 = 6;
                term2 = 16;
            }
            index = Count(figurePositions, mode[0] + "p") + term2;
            pos = new Cell(figurePositions.IndexOf(promoted[i])/2).name;
            if (promoted[i][1] == 'q' && Count(figurePositions, promoted[i]) > 1)
            {
                figures[index] = new Queen(pos[0], pos[1], mode, figurePrefabs[4 + term1], this, ids[mode[0] + "queen"]);
                figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                figurePositions = ReplaceFirst(figurePositions, mode[0] + "q", "NN");
                ids[mode[0] + "queen"]++;
            }
            else if (promoted[i][1] != 'q' && Count(figurePositions, promoted[i]) > 2)
            {
                switch (promoted[i][1])
                {
                    case 'k':
                        figures[index] = new Knight(pos[0], pos[1], mode, ids[mode[0] + "knight"], figurePrefabs[1 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "knight"]++;
                        figurePositions = ReplaceFirst(figurePositions, mode[0] + "k", "NN");
                        break;
                    case 'b':
                        figures[index] = new Bishop(pos[0], pos[1], mode, ids[mode[0] + "bishop"], figurePrefabs[2 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "bishop"]++;
                        figurePositions = ReplaceFirst(figurePositions, mode[0] + "b", "NN");
                        break;
                    case 'r':
                        figures[index] = new Rook(pos[0], pos[1], mode, ids[mode[0] + "rook"], figurePrefabs[3 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        figurePositions = ReplaceFirst(figurePositions, mode[0] + "r", "NN");
                        ids[mode[0] + "rook"]++;
                        break;
                }
            }
        }
        ids = new Dictionary<string, int>()
        {
            { "wpawn", 1 },
            { "wknight", 1 },
            { "wbishop", 1 },
            { "wrook", 1 },
            { "bpawn", 1 },
            { "bknight", 1 },
            { "bbishop", 1 },
            { "brook", 1 }
        };
        for (int i = 0; i < figurePositions.Length; i += 2)
        {
            if (figurePositions[i] == 'N')
            {
                continue;
            }
            else
            {
                pos = new Cell(i/2).name;
                if (figurePositions[i] == 'w')
                {   
                    mode = "white";
                    term1 = 0;
                    term2 = 0;
                }
                else
                {
                    mode = "black";
                    term1 = 6;
                    term2 = 16;
                }
                switch (figurePositions[i + 1])
                {
                    case 'p':
                        index = ids[mode[0] + "pawn"] + term2 - 1;
                        figures[index] = new Pawn(pos[0], pos[1], mode, ids[mode[0] + "pawn"], figurePrefabs[0 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "pawn"]++;
                        break;
                    case 'k':
                        index = 8 + ids[mode[0] + "knight"] + term2 - 1;
                        figures[index] = new Knight(pos[0], pos[1], mode, ids[mode[0] + "knight"], figurePrefabs[1 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "knight"]++;
                        break;
                    case 'b':
                        index = 10 + ids[mode[0] + "bishop"] + term2 - 1;
                        figures[index] = new Bishop(pos[0], pos[1], mode, ids[mode[0] + "bishop"], figurePrefabs[2 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "bishop"]++;
                        break;
                    case 'r':
                        index = 12 + ids[mode[0] + "rook"] + term2 - 1;
                        figures[index] = new Rook(pos[0], pos[1], mode, ids[mode[0] + "rook"], figurePrefabs[3 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        ids[mode[0] + "rook"]++;
                        break;
                    case 'q':
                        index = 14 + term2;
                        figures[index] = new Queen(pos[0], pos[1], mode, figurePrefabs[4 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        break;
                    case 'K':
                        index = 15 + term2;
                        figures[index] = new King(pos[0], pos[1], mode, figurePrefabs[5 + term1], this);
                        figures[index].obj = Instantiate(figures[index].obj, figures[index].obj.transform.position, Quaternion.identity);
                        break;
                    default:
                        break;
                }
            }
        }
        return figures;
    }

    private static bool EnPassant(Pawn pawn)
    {
        if (History.LastMove[0] != null)
        {
            if (History.LastMove[0][1] == 'p')
            {
                if (new Cell(History.LastMove[1]) - new Cell(History.LastMove[3]) == 2)
                {
                    return new Cell(History.LastMove[3]) - pawn.pos == 10;
                }
            }
        }
        return false;
    }

    private static int Count(string str, string value)
    {
        string str2 = str.Replace(value, "");
        return (str.Length - str2.Length) / value.Length;
    }

    private static string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
}