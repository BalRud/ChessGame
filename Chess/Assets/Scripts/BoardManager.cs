using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    public bool[,] AllowedMoves { set; get; }

    public Chessman[,] Chessmans { set; get; }
    public Chessman[,] TempChessmans { set; get; }
    public Chessman[,] CapturedBlackChessman { set; get; }
    public Chessman[,] CapturedWhiteChessman { set; get; }
    public Chessman SelectedChessman { set; get; }
    private Chessman selectedCapturedChessman;

    public List<GameObject> chessmanPrefabs;
    public List<GameObject> activeChessman;
    private List<GameObject> activeTempChessman;
    private List<GameObject> capturedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private Quaternion orientation = Quaternion.Euler(0, 90, 0);
    private Quaternion orientation2 = Quaternion.Euler(0, 270, 0);

    private int selectionX = -1;
    private int selectionY = -1;

    private Material previousMat;
    public Material selectedMat;

    public int[] EnPassantMove { set; get; }
    public bool WhiteKingMoved { set; get; }
    public bool BlackKingMoved { set; get; }
    public bool RookA1Moved { set; get; }
    public bool RookA8Moved { set; get; }
    public bool RookH1Moved { set; get; }
    public bool RookH8Moved { set; get; }

    private bool Check;

    public GameObject Canvas;
    public GameObject Menu;
    public GameObject ChoiceMenu;
    public GameObject PlayButton;
    public GameObject PlayAgainButton;
    public GameObject WhiteWin;
    public GameObject BlackWin;
    public GameObject Tie;
    public GameObject Pause;

    public Text Log;
    private Stack<string> GameLogs;
    private string[,] tileName;
    private string[] rowName;
    private string turn;
    private int turnCounter;

    public bool isWhiteTurn = true;

    private void Start()
    {
        Pause.SetActive(false);
        WhiteWin.SetActive(false);
        BlackWin.SetActive(false);
        Tie.SetActive(false);
        PlayAgainButton.SetActive(false);
        ChoiceMenu.SetActive(false);

        Instance = this;
        SpawnAllChessmans();
        EnPassantMove = new int[2] { -1, -1 };
        GameLogs = new Stack<string>();

        turnCounter = 1;
        rowName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        tileName = new string[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tileName[i, j] = rowName[i] + (j + 1);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (Pause.gameObject.activeInHierarchy == false)
            {
                Canvas.SetActive(true);
                Pause.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                Canvas.SetActive(false);
                Pause.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }

        DrawChessboard();
        DrawCapturedChessmenBoard();
        UpdateSelection();

        if (!EventSystem.current.IsPointerOverGameObject())
            if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (SelectedChessman == null)
                {
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    MoveChessman(selectionX, selectionY);
                }
            }
        }
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
            return;

        if (Chessmans[x, y].isWhite != isWhiteTurn)
            return;

        bool hasAtleastOneMove = false;
        //AllowedMoves = Chessmans[x, y].PossibleMove(Chessmans);
        PossibleMoves(x, y);
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (AllowedMoves[i, j])
                    hasAtleastOneMove = true;

        if (!hasAtleastOneMove)
            return;

        SelectedChessman = Chessmans[x, y];
        previousMat = SelectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        SelectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
        BoardHighlights.Instance.HighlightAllowedMoves(AllowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (AllowedMoves[x, y])
        {
            Chessman c = Chessmans[x, y];

            //Capture a piece

            if (c != null && c.isWhite != isWhiteTurn)
            {
                activeChessman.Remove(c.gameObject);
                CaptureChessman(c);
            }

            //EnPassantMove + Promotion
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                if (isWhiteTurn)
                {
                    c = Chessmans[x, y - 1];
                    Chessmans[x, y - 1] = null;
                }
                else
                {
                    c = Chessmans[x, y + 1];
                    Chessmans[x, y + 1] = null;
                }

                activeChessman.Remove(c.gameObject);
                CaptureChessman(c);
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;

            if (SelectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7 || y == 0)
                {
                    Canvas.SetActive(true);
                    ChoiceMenu.SetActive(true);
                }

                if (SelectedChessman.CurrentY == 1 && y == 3)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y - 1;
                }
                else if (SelectedChessman.CurrentY == 6 && y == 4)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y + 1;
                }
            }

            //CastlingMove

            if (SelectedChessman.GetType() == typeof(Rook))
            {
                if (SelectedChessman.CurrentX == 0 && SelectedChessman.CurrentY == 0)
                    RookA1Moved = true;
                else if (SelectedChessman.CurrentX == 7 && SelectedChessman.CurrentY == 0)
                    RookH1Moved = true;
                else if (SelectedChessman.CurrentX == 0 && SelectedChessman.CurrentY == 7)
                    RookA8Moved = true;
                else if (SelectedChessman.CurrentX == 7 && SelectedChessman.CurrentY == 7)
                    RookH8Moved = true;
            }

            if (SelectedChessman.GetType() == typeof(King))
            {
                if (isWhiteTurn)
                    WhiteKingMoved = true;
                else
                    BlackKingMoved = true;
            }

            if (SelectedChessman.GetType() == typeof(King))
            {
                if (SelectedChessman.CurrentX == 4 && x == 6)
                {
                    SelectedChessman = Chessmans[7, y];
                    Chessmans[7, y] = null;
                    SelectedChessman.transform.position = GetTileCenter(5, y);
                    SelectedChessman.SetPosition(5, y);
                    Chessmans[5, y] = SelectedChessman;
                    SelectedChessman = Chessmans[4, y];
                }
                if (SelectedChessman.CurrentX == 4 && x == 2)
                {
                    SelectedChessman = Chessmans[0, y];
                    Chessmans[0, y] = null;
                    SelectedChessman.transform.position = GetTileCenter(3, y);
                    SelectedChessman.SetPosition(3, y);
                    Chessmans[3, y] = SelectedChessman;
                    SelectedChessman = Chessmans[4, y];
                }
            }

            // Logs
            if (isWhiteTurn)
            {
                //Debug.Log("W:" + tileName[SelectedChessman.CurrentX, SelectedChessman.CurrentY] + "=>" + tileName[x, y] + ";");
                turn = turnCounter + ": ";
                turn += "W:" + tileName[SelectedChessman.CurrentX, SelectedChessman.CurrentY] + "=>" + tileName[x, y] + ";";
            }
            else
            {
                //Debug.Log("B:" + tileName[SelectedChessman.CurrentX, SelectedChessman.CurrentY] + "=>" + tileName[x, y]);
                turn += "B:" + tileName[SelectedChessman.CurrentX, SelectedChessman.CurrentY] + "=>" + tileName[x, y] + "\n";
                turnCounter++;
                GameLogs.Push(turn);
            }

            Log.text = " ";
            foreach (var i in GameLogs)
            {
                Log.text += i;
            }

            Chessmans[SelectedChessman.CurrentX, SelectedChessman.CurrentY] = null;
            SelectedChessman.transform.position = GetTileCenter(x, y);
            SelectedChessman.SetPosition(x, y);
            Chessmans[x, y] = SelectedChessman;

            if (HitTile(Chessmans))
            {
                //Debug.Log("CHECK");
                
                Check = true;
            }

            isWhiteTurn = !isWhiteTurn;
        }

        SelectedChessman.GetComponent<MeshRenderer>().material = previousMat;
        BoardHighlights.Instance.HideHighlights();
        SelectedChessman = null;

        if (CheckMateOrTie())
        {
            //Debug.Log(Check ? "CheckMate!" : "Tie");
            if (!isWhiteTurn && Check)
                WhiteWin.SetActive(true);
            else if (isWhiteTurn && Check)
                BlackWin.SetActive(true);
            else
                Tie.SetActive(true);

            EndGame();
        }

    }

    private void PossibleMoves(int x, int y)
    {
        activeTempChessman = new List<GameObject>();
        bool[,] possMoves = Chessmans[x, y].PossibleMove(Chessmans);

        for (int x1 = 0; x1 < 8; x1++)
        {
            for (int y1 = 0; y1 < 8; y1++)
            {
                if (possMoves[x1, y1])
                {
                    CopyChessmans();

                    SelectedChessman = TempChessmans[x, y];
                    TempChessmans[x, y] = null;
                    TempChessmans[x1, y1] = SelectedChessman;

                    isWhiteTurn = !isWhiteTurn;

                    if (HitTile(TempChessmans))
                        possMoves[x1, y1] = false;

                    isWhiteTurn = !isWhiteTurn;

                    foreach (var item in activeTempChessman)
                    {
                        if (item != null)
                        {
                            Destroy(item.gameObject);
                        }
                    }
                }
            }
        }
        AllowedMoves = possMoves;
    }                                   // Возможные ходы

    private bool HitTile(Chessman[,] Chessmans)
    {
        foreach (var chessman in Chessmans)
        {
            Chessman c;
            if (chessman != null && chessman.isWhite && isWhiteTurn)
            {
                bool[,] allowedMoves = chessman.PossibleMove(Chessmans);
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (allowedMoves[x, y])
                        {
                            c = Chessmans[x, y];
                            if (c != null && c.GetType() == typeof(King))
                                return true;
                        }
                    }
                }
            }
            else if (chessman != null && !chessman.isWhite && !isWhiteTurn)
            {
                bool[,] allowedMoves = chessman.PossibleMove(Chessmans);
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (allowedMoves[x, y])
                        {
                            c = Chessmans[x, y];
                            if (c != null && c.GetType() == typeof(King))
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }                                // Проверка есть ли шах

    private bool CheckMateOrTie()
    {
        bool b = true;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Chessman c = Chessmans[x, y];
                if (c != null && c.isWhite == isWhiteTurn)
                {
                    PossibleMoves(x, y);
                    for (int x1 = 0; x1 < 8; x1++)
                        for (int y1 = 0; y1 < 8; y1++)
                            if (AllowedMoves[x1, y1])
                            {
                                b = false;
                            }
                }
            }
        }

        return b;
    }                                              // Проверка на ничью или мат  

    public bool CastlingMoveShortCheck(int x, int y)
    {
        Chessman c;
        for (int x1 = 0; x1 < 8; x1++)
        {
            for (int y1 = 0; y1 < 8; y1++)
            {
                c = Chessmans[x, y];
                if (c != null && !c.isWhite == isWhiteTurn)
                {
                    bool[,] allowedMoves = c.PossibleMove(Chessmans);
                    if (allowedMoves[x, y] || allowedMoves[x + 1, y] || allowedMoves[x + 2, y] || allowedMoves[x + 3, y])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }                           // Проверка возможности короткой рокировки

    public bool CastlingMoveLongCheck(int x, int y)
    {
        Chessman c;
        for (int x1 = 0; x1 < 8; x1++)
        {
            for (int y1 = 0; y1 < 8; y1++)
            {
                c = Chessmans[x, y];
                if (c != null && !c.isWhite == isWhiteTurn)
                {
                    bool[,] allowedMoves = c.PossibleMove(Chessmans);
                    if (allowedMoves[x, y] || allowedMoves[x + 1, y] || allowedMoves[x - 2, y] || allowedMoves[x - 3, y] || allowedMoves[x - 4, y])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }                            // Проверка возможности длинной рокировки

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void CaptureChessman(Chessman c)
    {
        capturedChessman.Add(c.gameObject);
        if (c.isWhite)
        {
            for (int x1 = 0; x1 <= 1; x1++)
            {
                for (int y1 = 7; y1 >= 0; y1--)
                {
                    if (CapturedWhiteChessman[x1, y1] == null && c != null)
                    {
                        c.transform.position = WhiteCapturedGetTileCenter(x1, y1);
                        CapturedWhiteChessman[x1, y1] = c.GetComponent<Chessman>();
                        CapturedWhiteChessman[x1, y1].SetPosition(x1, y1);
                        c = null;
                        break;
                    }
                }
            }
            SortCapturedWhiteChessman();
        }
        else if (!c.isWhite)
        {
            for (int x1 = 1; x1 >= 0; x1--)
            {
                for (int y1 = 0; y1 <= 7; y1++)
                {
                    if (CapturedBlackChessman[x1, y1] == null && c != null)
                    {
                        c.transform.position = BlackCapturedGetTileCenter(x1, y1);
                        CapturedBlackChessman[x1, y1] = c.GetComponent<Chessman>();
                        CapturedBlackChessman[x1, y1].SetPosition(x1, y1);
                        c = null;
                        break;
                    }
                }
            }
            SortCapturedBlackChessman();
        }
    }

    private void SortCapturedWhiteChessman()
    {
        for (int x = 1; x >= 0; x--)
        {
            for (int y = 0; y < 7; y++)
            {
                Chessman Chessman1 = CapturedWhiteChessman[x, y];
                Chessman Chessman2 = CapturedWhiteChessman[x, y + 1];
                if (Chessman1 != null && Chessman2 != null)
                {
                    if (Chessman1.weight < Chessman2.weight)
                    {
                        selectedCapturedChessman = CapturedWhiteChessman[x, y];
                        CapturedWhiteChessman[x, y] = null;
                        selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(1, 0);
                        selectedCapturedChessman.SetPosition(1, 0);
                        CapturedWhiteChessman[1, 0] = selectedCapturedChessman;

                        selectedCapturedChessman = CapturedWhiteChessman[x, y + 1];
                        CapturedWhiteChessman[x, y + 1] = null;
                        selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(x, y);
                        selectedCapturedChessman.SetPosition(x, y);
                        CapturedWhiteChessman[x, y] = selectedCapturedChessman;

                        selectedCapturedChessman = CapturedWhiteChessman[1, 0];
                        CapturedWhiteChessman[1, 0] = null;
                        selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(x, y + 1);
                        selectedCapturedChessman.SetPosition(x, y + 1);
                        CapturedWhiteChessman[x, y + 1] = selectedCapturedChessman;


                    }
                }
                if (CapturedWhiteChessman[1, 7] != null && CapturedWhiteChessman[0, 0] != null &&
                        CapturedWhiteChessman[1, 7].weight < CapturedWhiteChessman[0, 0].weight)
                {
                    selectedCapturedChessman = CapturedWhiteChessman[1, 7];
                    CapturedWhiteChessman[1, 7] = null;
                    selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(1, 0);
                    selectedCapturedChessman.SetPosition(1, 0);
                    CapturedWhiteChessman[1, 0] = selectedCapturedChessman;

                    selectedCapturedChessman = CapturedWhiteChessman[0, 0];
                    CapturedWhiteChessman[0, 0] = null;
                    selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(1, 7);
                    selectedCapturedChessman.SetPosition(1, 7);
                    CapturedWhiteChessman[1, 7] = selectedCapturedChessman;

                    selectedCapturedChessman = CapturedWhiteChessman[1, 0];
                    CapturedWhiteChessman[1, 0] = null;
                    selectedCapturedChessman.transform.position = WhiteCapturedGetTileCenter(0, 0);
                    selectedCapturedChessman.SetPosition(0, 0);
                    CapturedWhiteChessman[0, 0] = selectedCapturedChessman;
                }
            }
        }
    }

    private void SortCapturedBlackChessman()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 7; y >= 1; y--)
            {
                Chessman Chessman1 = CapturedBlackChessman[x, y];
                Chessman Chessman2 = CapturedBlackChessman[x, y - 1];
                if (Chessman1 != null && Chessman2 != null)
                {
                    if (Chessman1.weight < Chessman2.weight)
                    {
                        selectedCapturedChessman = CapturedBlackChessman[x, y];
                        CapturedBlackChessman[x, y] = null;
                        selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(0, 7);
                        selectedCapturedChessman.SetPosition(0, 7);
                        CapturedBlackChessman[0, 7] = selectedCapturedChessman;

                        selectedCapturedChessman = CapturedBlackChessman[x, y - 1];
                        CapturedBlackChessman[x, y - 1] = null;
                        selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(x, y);
                        selectedCapturedChessman.SetPosition(x, y);
                        CapturedBlackChessman[x, y] = selectedCapturedChessman;

                        selectedCapturedChessman = CapturedBlackChessman[0, 7];
                        CapturedBlackChessman[0, 7] = null;
                        selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(x, y - 1);
                        selectedCapturedChessman.SetPosition(x, y - 1);
                        CapturedBlackChessman[x, y - 1] = selectedCapturedChessman;
                    }
                }
                if (CapturedBlackChessman[0, 0] != null && CapturedBlackChessman[1, 7] != null &&
                        CapturedBlackChessman[0, 0].weight < CapturedBlackChessman[1, 7].weight)
                {
                    selectedCapturedChessman = CapturedBlackChessman[0, 0];
                    CapturedBlackChessman[0, 0] = null;
                    selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(0, 7);
                    selectedCapturedChessman.SetPosition(0, 7);
                    CapturedBlackChessman[0, 7] = selectedCapturedChessman;

                    selectedCapturedChessman = CapturedBlackChessman[1, 7];
                    CapturedBlackChessman[1, 7] = null;
                    selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(0, 0);
                    selectedCapturedChessman.SetPosition(0, 0);
                    CapturedBlackChessman[0, 0] = selectedCapturedChessman;

                    selectedCapturedChessman = CapturedBlackChessman[0, 7];
                    CapturedBlackChessman[0, 7] = null;
                    selectedCapturedChessman.transform.position = BlackCapturedGetTileCenter(1, 7);
                    selectedCapturedChessman.SetPosition(1, 7);
                    CapturedBlackChessman[1, 7] = selectedCapturedChessman;
                }
            }
        }
    }

    private void CopyChessmans()
    {
        Chessman c;
        TempChessmans = new Chessman[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                c = Chessmans[x, y];
                if (c)
                {
                    if (c.GetType() == typeof(Rook))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(2, x, y);
                        else
                            SpawnTempChessman(8, x, y);
                    }
                    else if (c.GetType() == typeof(Knight))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(4, x, y);
                        else
                            SpawnTempChessman(10, x, y);
                    }
                    else if (c.GetType() == typeof(Bishop))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(3, x, y);
                        else
                            SpawnTempChessman(9, x, y);
                    }
                    else if (c.GetType() == typeof(Queen))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(1, x, y);
                        else
                            SpawnTempChessman(7, x, y);
                    }
                    else if (c.GetType() == typeof(King))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(0, x, y);
                        else
                            SpawnTempChessman(6, x, y);
                    }
                    else if (c.GetType() == typeof(Pawn))
                    {
                        if (c.isWhite)
                            SpawnTempChessman(5, x, y);
                        else
                            SpawnTempChessman(11, x, y);
                    }

                }
            }
        }
    }

    private void SpawnTempChessman(int index, int x, int y)
    {
        Vector3 pos = Vector3.down * 10;
        GameObject go = Instantiate(chessmanPrefabs[index], pos, orientation) as GameObject;
        go.transform.SetParent(transform);
        TempChessmans[x, y] = go.GetComponent<Chessman>();
        TempChessmans[x, y].SetPosition(x, y);
        activeTempChessman.Add(go);
    }

    #region Creating board & chessmen
    private void DrawChessboard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heigthLine = Vector3.forward * 8;
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
            }
        }
    }

    private void DrawCapturedChessmenBoard()
    {
        Vector3 right = Vector3.right * 9;
        Vector3 widthLine = Vector3.right * 2;
        Vector3 heigthLine = Vector3.forward * 8;
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            for (int j = 0; j <= 2; j++)
            {
                start = Vector3.right * j;
            }
        }

        Vector3 left = Vector3.left * 3;
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            for (int j = 0; j <= 2; j++)
            {
                start = Vector3.right * j;
            }
        }
    }

    public void SpawnChessman(int index, int x, int y)
    {
        if (index == 4)
        {
            GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), orientation) as GameObject;
            go.transform.SetParent(transform);
            Chessmans[x, y] = go.GetComponent<Chessman>();
            Chessmans[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }
        else if (index == 10)
        {
            GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), orientation2) as GameObject;
            go.transform.SetParent(transform);
            Chessmans[x, y] = go.GetComponent<Chessman>();
            Chessmans[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }
        else
        {
            GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            Chessmans[x, y] = go.GetComponent<Chessman>();
            Chessmans[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }
    }

    public void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        capturedChessman = new List<GameObject>();
        CapturedWhiteChessman = new Chessman[2, 8];
        CapturedBlackChessman = new Chessman[2, 8];

        #region    SpawnChessmans     
        // White team

        //King
        SpawnChessman(0, 4, 0);

        //Queen
        SpawnChessman(1, 3, 0);

        //Rooks
        SpawnChessman(2, 0, 0);
        SpawnChessman(2, 7, 0);

        //Bishops
        SpawnChessman(3, 2, 0);
        SpawnChessman(3, 5, 0);

        //Knights
        SpawnChessman(4, 1, 0);
        SpawnChessman(4, 6, 0);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }

        // Black team

        //King
        SpawnChessman(6, 4, 7);

        //Queen
        SpawnChessman(7, 3, 7);

        //Rooks
        SpawnChessman(8, 0, 7);
        SpawnChessman(8, 7, 7);

        //Bishops
        SpawnChessman(9, 2, 7);
        SpawnChessman(9, 5, 7);

        //Knights
        SpawnChessman(10, 1, 7);
        SpawnChessman(10, 6, 7);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }

        #endregion     
    }
    #endregion

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private Vector3 WhiteCapturedGetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.right * 9;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        origin.y += -0.198f;
        return origin;
    }

    private Vector3 BlackCapturedGetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.left * 3;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        origin.y += -0.198f;
        return origin;
    }

    private void EndGame()
    {
        Canvas.SetActive(true);
        Menu.SetActive(true);
        PlayAgainButton.SetActive(true);
    }

    public void StartAgain()
    {
        foreach (GameObject go in activeChessman)
            Destroy(go);

        foreach (GameObject go in capturedChessman)
            Destroy(go);

        Log.text = " ";
        GameLogs.Clear();
        turnCounter = 1;

        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllChessmans();
    }
}
