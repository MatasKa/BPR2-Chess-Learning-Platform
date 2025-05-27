using System.Collections.Generic;
using UnityEngine;

public class MoveHistory : MonoBehaviour
{
    MoveTranslator moveTranslator = new MoveTranslator();
    private List<string> moves = new List<string>();

    public void AddMove(string move)
    {
        moves.Add(move);
        Debug.Log(string.Join(", ", moves));
    }
    public void ClearHistory()
    {
        moves.Clear();
    }
    public List<string> GetMoves()
    {
        return moves;
    }
    public string TranslateMoveToUci(Vector2Int FromSquare, Vector2Int ToSquare)
    {
        return moveTranslator.SquareToUci(FromSquare) + moveTranslator.SquareToUci(ToSquare);
    }
    public Vector2Int TranslatePositionToSquare(string square)
    {
        return moveTranslator.UciToSquare(square);
    }
    public void RebuildTensor(float[,,] tensor, System.Action<float[,,], string> applyMove, string startFEN, System.Func<string, float[,,]> fenToTensor)
    {
        var rebuilt = fenToTensor(startFEN);
        foreach (var move in moves)
            applyMove(rebuilt, move);

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                for (int p = 0; p < 12; p++)
                    tensor[r, c, p] = rebuilt[r, c, p];
    }
}
