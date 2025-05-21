using UnityEngine;
using System;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    [Tooltip("Drag in your ChessAI component here")]
    public ChessAI ai;

    private float[,,] currentTensor;
    private const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    [SerializeField] private MoveHistory moveHistory;

    void Start()
    {
        if (ai == null)
        {
            Debug.LogError("Please assign the ChessAI component in the Inspector!");
            enabled = false;
            return;
        }

        currentTensor = FENToTensor(START_FEN);

        // Let the AI pick its first move
        string first = ai.PredictMove(currentTensor);
        Debug.Log($"AI first move: {first}");

        ApplyMove(first);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Rebuild the tensor from move history
            moveHistory.RebuildTensor(currentTensor, ApplyUciMove, START_FEN, FENToTensor);

            // Now predict the move based on the full current state
            string next = ai.PredictMove(currentTensor);
            Debug.Log($"AI next move: {next}");

            // Apply and record the move
            ApplyMove(next);
        }

        // Optional: manual tensor rebuild (for debugging)
        if (Input.GetKeyDown(KeyCode.R))
        {
            moveHistory.RebuildTensor(currentTensor, ApplyUciMove, START_FEN, FENToTensor);
            Debug.Log("Tensor rebuilt from history.");
        }
    }


    void ApplyMove(string uciMove)
    {
        //ApplyUciMove(currentTensor, uciMove);
        moveHistory.AddMove(uciMove);
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
