using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AIController : MonoBehaviour
{
    //[Tooltip("Drag in your ChessAI component here")]
    public ChessAI ai;

    private float[,,] currentTensor;
    private const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    [SerializeField] private MoveHistory moveHistory;
    [SerializeField] private Board board;


    void Start()
    {
        if (ai == null)
        {
            Debug.LogError("Please assign the ChessAI component in the Inspector!");
            enabled = false;
            return;
        }

        currentTensor = FENToTensor(START_FEN);

        //string first = ai.PredictMove(currentTensor);
        if (board.turnManager.IsPlayerWhite() == false)
        {
            DoPredictedMove();
        }
    }

    public void DoPredictedMove()
    {
        StartCoroutine(DoPredictedMoveCoroutine());
    }
    public IEnumerator DoPredictedMoveCoroutine()
    {
        while (true)
        {
            if (moveHistory != null)
            {
                moveHistory.RebuildTensor(currentTensor, ApplyUciMove, START_FEN, FENToTensor);
            }
            string[] topMovesRaw = ai.PredictTopMoves(currentTensor, 1500); //2184

            // Deduplicate and filter invalid-length moves
            var topMoves = topMovesRaw
                .Where(m => m.Length >= 4)
                .Distinct()
                .ToList();

            if (topMoves.Count == 0)
            {
                Debug.LogWarning("AI prediction returned no valid moves. Retrying...");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Debug.Log($"AI predicted top moves: {string.Join(", ", topMoves)}");

            bool played = false;
            foreach (string move in topMoves)
            {
                var from = moveHistory.TranslatePositionToSquare(move.Substring(0, 2));
                var to = moveHistory.TranslatePositionToSquare(move.Substring(2, 2));
                Debug.LogWarning("About To check if move is legal " + board.gameObject.name);
                if (board.IsAIMoveLegal(from, to))
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));

                    Debug.Log($"AI plays move: {move}");
                    board.MoveAIPiece(from, to);
                    played = true;
                    break;
                }
                else
                {
                    Debug.Log($"AI move {move} was illegal");
                }
            }

            if (played)
                yield break;

            Debug.LogWarning("None of the AI's top predicted moves were legal. Retrying...");
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ApplyUciMove(float[,,] tensor, string uci)
    {
        int FileToCol(char f) => f - 'a';
        int RankToRow(char r) => r - '1';

        int fromCol = FileToCol(uci[0]);
        int fromRow = RankToRow(uci[1]);
        int toCol = FileToCol(uci[2]);
        int toRow = RankToRow(uci[3]);

        int plane = -1;
        for (int p = 0; p < 12; p++)
        {
            if (tensor[fromRow, fromCol, p] == 1f)
            {
                plane = p;
                break;
            }
        }

        if (plane < 0)
        {
            //Debug.LogError($"No piece found at {uci.Substring(0, 2)} to move!");
            return;
        }

        tensor[fromRow, fromCol, plane] = 0f;

        if (uci.Length == 5)
        {
            char promo = uci[4]; // e.g., 'q'
            var promoMap = new Dictionary<char, int> {
                {'q', 4}, {'r', 3}, {'b', 2}, {'n', 1}
            };
            bool isWhite = plane < 6;
            int basePlane = isWhite ? 0 : 6;
            plane = basePlane + promoMap[promo];
        }

        tensor[toRow, toCol, plane] = 1f;
    }

    private float[,,] FENToTensor(string fen)
    {
        var planes = new Dictionary<char, int> {
            {'P',0}, {'N',1}, {'B',2}, {'R',3}, {'Q',4}, {'K',5},
            {'p',6}, {'n',7}, {'b',8}, {'r',9}, {'q',10},{'k',11}
        };

        var tensor = new float[8, 8, 12];
        string[] parts = fen.Split(' ');
        string[] ranks = parts[0].Split('/');

        for (int r = 0; r < 8; r++)
        {
            string rank = ranks[7 - r];
            int file = 0;
            foreach (char c in rank)
            {
                if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    int p = planes[c];
                    tensor[r, file, p] = 1f;
                    file++;
                }
            }
        }
        return tensor;
    }
}
