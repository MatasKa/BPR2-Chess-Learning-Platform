using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class King : Piece
{
    public override void PossibleMoves()
    {
        base.PossibleMoves();
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

        //Add more checks that don't allow to enter a square that is "possible" by opponent

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = base.currentSquare + dir;
            if (board.IsInsideBoard(newPos))
            {
                Piece temp = null;
                temp = board.GetPieceOnSquare(newPos);
                if (temp == null)
                {
                    board.Highlight(newPos);
                }
                else if (board.IsEnemyPiece(temp) == true)
                {
                    board.Highlight(newPos);
                }
            }
        }
    }
}
