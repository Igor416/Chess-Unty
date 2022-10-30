using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Offline : MonoBehaviour
{
    //prefabs
    public GameObject chessboard;
    public GameObject menu, pawnMenu;
    public GameObject[] prefabs = new GameObject[12];

    //sprites
    public Sprite[] whiteFigures = new Sprite[4];
    public Sprite[] blackFigures = new Sprite[4];

    //stats
    private Figure choosedFigure;
    private int figureIndex;

    //labels { labelName: text }
    private readonly Dictionary<string, Text> labels = new Dictionary<string, Text>();
    private readonly string[] texts = new string[]
    {
        "Move", "Round"
    };

    private readonly Dictionary<string, int> ids = new Dictionary<string, int>()
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

    private Desk desk;
    private bool isPause;
    private string mode;
    private int round;

    void Start()
    {
        desk = chessboard.GetComponent<Desk>();
        menu.GetComponent<Image>().enabled = false;
        foreach (var component in menu.GetComponentsInChildren<Text>())
        {
            component.enabled = false;
        }
        foreach (var component in menu.GetComponentsInChildren<Image>())
        {
            component.enabled = false;
        }

        pawnMenu.GetComponent<Image>().enabled = false;
        foreach (var component in pawnMenu.GetComponentsInChildren<Text>())
        {
            component.enabled = false;
        }
        foreach (var component in pawnMenu.GetComponentsInChildren<Image>())
        {
            component.enabled = false;
        }

        foreach (string text in texts)
        {
            labels.Add(text, GameObject.Find(text).GetComponent<Text>());
        }

        mode = "white";
        round = 1;

        UpdateText();

        (string figures, string mode, int round) loaded = History.Load(1);
        if (loaded.round != -1)
        {
            mode = loaded.mode;
            round = loaded.round;
        }
        desk.figures = desk.CreateFigures(loaded.figures, prefabs);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Cell cell = new Cell(pos.x, pos.y);
            if (desk.ThereIsADot(cell))
            {
                Figure figure = desk.GetFigureBy(choosedFigure.name);
                desk.Move(cell, figure, mode, round);
                if (figure.name[1] == 'p')
                {
                    if (((figure as Pawn).direction == 1 && figure.pos.yIndex == 7) || ((figure as Pawn).direction == -1 && figure.pos.yIndex == 0)) //direction is hiden, so here downcast is not for beauty
                    {
                        for (int i = 0; i < desk.figures.Length; i++)
                        {
                            if (desk.figures[i] != null && figure.name == desk.figures[i].name)
                            {
                                figureIndex = i;
                                OpenPawnMenu();
                            }
                        }
                    }
                }
                ChangeMode();
                round++;
                UpdateText();
            }
            else if (desk.ThereIsAFigure(cell))
            {
                Figure figure = desk.GetFigureBy(cell);
                desk.ShowMoves(figure, mode, round);
                choosedFigure = figure;
            }
        }
    }
    //button functions
    public void Knight() { PromotePawn("knight"); }
    public void Bishop() { PromotePawn("bishop"); }
    public void Rook() { PromotePawn("rook"); }
    public void Queen() { PromotePawn("queen"); }

    public void OpenMenu()
    {
        bool status;
        if (isPause)
        {
            status = false;
        }
        else
        {
            status = true;
        }
        menu.GetComponent<Image>().enabled = status;
        foreach (var component in menu.GetComponentsInChildren<Text>())
        {
            component.enabled = status;
        }
        foreach (var component in menu.GetComponentsInChildren<Image>())
        {
            component.enabled = status;
        }
        isPause = status;
    }

    public void OpenPawnMenu()
    {
        isPause = true;
        pawnMenu.GetComponent<Image>().enabled = true;
        foreach (var component in pawnMenu.GetComponentsInChildren<Text>())
        {
            component.enabled = true;
        }
        for (int i = 1; i < pawnMenu.GetComponentsInChildren<Image>().Length; i++)
        {
            if (mode == "white")
            {
                pawnMenu.GetComponentsInChildren<Image>()[i].sprite = whiteFigures[i - 1];
            }
            else
            {
                pawnMenu.GetComponentsInChildren<Image>()[i].sprite = blackFigures[i - 1];
            }
            pawnMenu.GetComponentsInChildren<Image>()[i].enabled = true;
        }
    }

    public void Restart()
    {
        chessboard.GetComponent<Desk>().DeleteFigures();
        chessboard.GetComponent<Desk>().CreateFigures(null, prefabs);
        History.DeleteLastMoves();
        round = 1;
        mode = "white";
    }

    public void MoveBack()
    {
        Figure figure1;
        string cell1 = History.LastMove[1];
        Figure figure2;
        if (round > 1)
        {
            Figure promoted = desk.GetFigureBy(History.LastMove[3]);
            if (promoted != null)
            {
                ChangeMode();

                if (mode == "white")
                {
                    figure1 = new Pawn(cell1[0], cell1[1], mode, History.LastMove[0][2] - '0', prefabs[0], desk);
                }
                else
                {
                    figure1 = new Pawn(cell1[0], cell1[1], mode, History.LastMove[0][2] - '0', prefabs[5], desk);
                }
                figure1.obj = GameObject.Find($"{figure1.name[0]}p(Clone)");
                figure1.Move(new Cell(cell1));
                for (int i = 0; i < desk.figures.Length; i++)
                {
                    if (promoted.name == desk.figures[i].name)
                    {
                        desk.figures[i] = figure1;
                    }
                }

                figure2 = desk.GetFigureBy(History.LastMove[2]);
                figure2.Beat(promoted);
                switch (figure2.name[1])
                {
                    case 'k':
                        ids[$"{mode[0]}knight"]--;
                        break;
                    case 'b':
                        ids[$"{mode[0]}bishop"]--;
                        break;
                    case 'r':
                        ids[$"{mode[0]}rook"]--;
                        break;
                    case 'q':
                        ids[$"{mode[0]}queen"]--;
                        break;
                }
                Destroy(promoted.obj);
                ChangeMode();
            }
            else
            {

                figure1 = desk.GetFigureBy(History.LastMove[0]);
                figure2 = desk.GetFigureBy(History.LastMove[2]);
                Cell cell2 = new Cell(History.LastMove[3]);
                figure1.Move(new Cell(cell1));

                if (figure2 != null)
                {
                    figure2.Move(cell2);
                }
            }
            History.MoveBack();
            ChangeMode();
            round--;

            desk.DeleteDots();
            return;
        }
    }

    public void Save()
    {
        History.Save(mode, round, TransformFiguresPositionsToString(desk.figures));
    }

    private void ChangeMode()
    {
        if (mode == "white")
        {
            mode = "black";
        }
        else
        {
            mode = "white";
        }
    }

    private void PromotePawn(string figureName)
    {
        Figure figure = desk.figures[figureIndex];
        char x = figure.pos.name[0];
        char y = figure.pos.name[1];
        ChangeMode();
        Figure newFigure;

        int term;
        if (mode == "white") term = 0;
        else term = 6;

        switch (figureName)
        {
            case "knight":
                newFigure = new Knight(x, y, mode, ids[$"{mode[0]}knight"], prefabs[1 + term], desk);
                break;
            case "bishop":
                newFigure = new Bishop(x, y, mode, ids[$"{mode[0]}bishop"], prefabs[2 + term], desk);
                break;
            case "rook":
                newFigure = new Rook(x, y, mode, ids[$"{mode[0]}rook"], prefabs[3 + term], desk);
                break;
            case "queen":
                newFigure = new Queen(x, y, mode, prefabs[4 + term], desk, ids[$"{mode[0]}queen"]);
                break;
            default:
                newFigure = null;
                break;
        }

        desk.figures[figureIndex].pos = new Cell();
        desk.figures[figureIndex].obj.transform.position = new Vector2(100f, 100f);

        desk.figures[figureIndex] = newFigure;
        newFigure.obj = Instantiate(newFigure.obj, newFigure.obj.transform.position, Quaternion.identity);

        string lastPawnPlace = History.LastMove[1];
        string enemy = History.LastMove[2];
        History.MoveBack();

        History.SetLastMove(figure.name, lastPawnPlace, enemy, newFigure.name);
        ChangeMode();
        ids[mode[0] + figureName]++;

        pawnMenu.GetComponent<Image>().enabled = false;
        foreach (var component in pawnMenu.GetComponentsInChildren<Text>())
        {
            component.enabled = false;
        }
        foreach (var component in pawnMenu.GetComponentsInChildren<Image>())
        {
            component.enabled = false;
        }
    }

    private static string[] TransformFiguresPositionsToString(Figure[] figures)
    {
        string[] arr = new string[128];
        Cell pos;
        string name;
        for (int i = 0; i < figures.Length; i++)
        {
            if (figures[i] != null)
            {
                pos = figures[i].pos;
                if (pos.xIndex != -1 && pos.yIndex != -1)
                {
                    name = figures[i].name;
                    arr[(pos.xIndex + pos.yIndex * 8) * 2] = name[0].ToString();
                    arr[(pos.xIndex + pos.yIndex * 8) * 2 + 1] = name[1].ToString();
                }
            }
        }
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] is null)
            {
                arr[i] = "N";
            }
        }
        return arr;
    }

    private static string Title(string str)
    {
        string upper = str.ToUpper();
        string title = upper[0].ToString();
        for (int i = 1; i < str.Length; i++)
        {
            title += str[i];
        }
        return title;
    }

    private void UpdateText()
    {
        labels["Move"].text = Title(mode) + " move";
        labels["Round"].text = "round " + round.ToString();

        if (desk.IsCheck() && desk.check != null)
        {
            if (desk.IsMate(mode))
            {
                desk.ShowCheck("Mate!");
            }
            else
            {
                desk.ShowCheck("Check!");
            }
        }
    }
}
