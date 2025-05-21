using UnityEngine;
using System;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    [Tooltip("Drag in your ChessAI component here")]
    public ChessAI ai;

    // The board as an [8,8,12] tensor, updated each move
    private float[,,] currentTensor;

    // Standard chess starting position FEN
    private const string START_FEN =
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    void Start()
    {
        if (ai == null)
        {
            Debug.LogError("Please assign the ChessAI component in the Inspector!");
            enabled = false;
            return;
        }

        // Initialize tensor from the starting FEN
        currentTensor = FENToTensor(START_FEN);

        // Let the AI pick its very first move and log it
        string first = ai.PredictMove(currentTensor);
        Debug.Log($"AI first move: {first}");

        // Apply it so currentTensor now reflects that move
        ApplyUciMove(currentTensor, first);
    }

    void Update()
    {
        // If the player presses 'A', get the AI's next move on the updated board
        if (Input.GetKeyDown(KeyCode.A))
        {
            string next = ai.PredictMove(currentTensor);
            Debug.Log($"AI next move: {next}");
            ApplyUciMove(currentTensor, next);
        }
    }

    /// <summary>
    /// Applies a UCI move (e.g. "e2e4" or "a7a8q") to the given board tensor in‐place.
    /// </summary>
    void ApplyUciMove(float[,,] tensor, string uci)
    {
        // helper to map FEN ranks/files → tensor row/col
        int FileToCol(char f) => f - 'a';
        int RankToRow(char r) => r - '1';

        int fromCol = FileToCol(uci[0]);
        int fromRow = RankToRow(uci[1]);
        int toCol = FileToCol(uci[2]);
        int toRow = RankToRow(uci[3]);

        // 1) Find which plane holds the moving piece
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
            Debug.LogError($"No piece found at {uci.Substring(0, 2)} to move!");
            return;
        }

        // 2) Clear the origin
        tensor[fromRow, fromCol, plane] = 0f;

        // 3) Handle promotions (uci length == 5)
        if (uci.Length == 5)
        {
            // remove the pawn from origin already done; now pick new plane
            char promo = uci[4]; // 'q','r','b','n'
            var promoMap = new Dictionary<char, int> {
                {'q', 4}, {'r', 3}, {'b', 2}, {'n', 1}
            };
            bool isWhite = plane < 6;
            int basePlane = isWhite ? 0 : 6;
            plane = basePlane + promoMap[promo];
        }

        // 4) Place the piece at the destination
        tensor[toRow, toCol, plane] = 1f;
    }

    /// <summary>
    /// Converts a FEN string into a [8,8,12] float tensor:
    ///   - ranks '1'→row7 becomes tensor row0, so we invert via 8 - rank 
    ///   - files 'a'→col0, … 'h'→col7
    ///   - planes P,N,B,R,Q,K → 0–5, p,n,b,r,q,k → 6–11
    /// </summary>
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
            // invert so rank '1' (ranks[7]) → row 0
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
