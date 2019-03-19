using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { get; set; }

    public GameObject highlightPrefab;
    public GameObject highligtHitPrefab;
    private List<GameObject> highlights;
    private List<GameObject> highlightsHit;

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        highlightsHit = new List<GameObject>();
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }
        return go;
    }

    private GameObject GetHighlightHitObject()
    {
        GameObject go = highlightsHit.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highligtHitPrefab);
            highlightsHit.Add(go);
        }
        return go;
    }

    public void HighlightAllowedMoves(bool[,] allowedMoves)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (allowedMoves[x, y])
                {
                    BoardManager BM = BoardManager.Instance;
                    if (BM.Chessmans[x, y] == null)
                    {
                        if (BM.SelectedChessman.GetType() == typeof(Pawn) && PawnCheck(x, y))
                        {
                            GameObject highligtHit = GetHighlightHitObject();
                            highligtHit.SetActive(true);
                            highligtHit.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
                        }
                        GameObject highligt = GetHighlightObject();
                        highligt.SetActive(true);
                        highligt.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
                    }
                    else if (BM.Chessmans[x, y] != null)
                    {
                        GameObject highligthit = GetHighlightHitObject();
                        highligthit.SetActive(true);
                        highligthit.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
                    }
                }
            }
        }
    }

    public bool PawnCheck(int x, int y)
    {
        BoardManager BM = BoardManager.Instance;
        //White pawn
        if (BM.isWhiteTurn)
        {
            if (x < 7 && BM.Chessmans[x + 1, y - 1] == BM.SelectedChessman || x > 0 && BM.Chessmans[x - 1, y - 1] == BM.SelectedChessman)
            {
                return true;
            }
        }
        //Black pawn
        if (!BM.isWhiteTurn)
        {
            if (x < 7 && BM.Chessmans[x + 1, y + 1] == BM.SelectedChessman || x > 0 && BM.Chessmans[x - 1, y + 1] == BM.SelectedChessman)
            {
                return true;
            }
        }
        return false;
    }

    public void HideHighlights()
    {
        foreach (var item in highlights)
        {
            item.SetActive(false);
        }

        foreach (var item in highlightsHit)
        {
            item.SetActive(false);
        }
    }
}
