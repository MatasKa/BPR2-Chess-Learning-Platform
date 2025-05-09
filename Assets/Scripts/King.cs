using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(+0, +1),
        new Vector2Int(+1, +1),
        new Vector2Int(+1, 0),
        new Vector2Int(+1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, +1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = base.currentSquare + dir;

            if (board.IsInsideBoard(newPos))
            {
                Piece temp = board.GetPieceOnSquare(newPos);
                if (temp == null || board.IsEnemyPiece(this, temp))
                {
                    moves.Add(newPos);
                }
            }
        }

        return moves;
    }
}
