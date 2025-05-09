using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    private bool castled = false;
    //Don't forget to check castling!!!

    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            for (int o = 1; o <= 8; o++)
            {
                Vector2Int newPos = base.currentSquare + directions[i] * o;
                if (board.IsInsideBoard(newPos))
                {
                    Piece temp = board.GetPieceOnSquare(newPos);
                    if (temp == null)
                    {
                        moves.Add(newPos);
                    }
                    else
                    {
                        if (board.IsEnemyPiece(this, temp))
                            moves.Add(newPos);
                        break;
                    }
                }
            }
        }

        return moves;
    }
}
