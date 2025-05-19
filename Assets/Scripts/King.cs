using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private bool hasMoved = false;
    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = StandartMoves();

        if (hasMoved == false)
        {
            //castling 0-0
            if (board.GetPieceOnSquare(currentSquare + new Vector2Int(1, 0)) == null
             && board.GetPieceOnSquare(currentSquare + new Vector2Int(2, 0)) == null
             && board.specialMoveChecker.HasRookMoved(board.GetPieceOnSquare(currentSquare + new Vector2Int(3, 0))) == false
            && board.specialMoveChecker.PassesCheckKingsideCastle(white) == false)
            {
                moves.Add(new Vector2Int(6, currentSquare.y));
            }
            //castling 0-0-0
            if (board.GetPieceOnSquare(currentSquare + new Vector2Int(-1, 0)) == null
             && board.GetPieceOnSquare(currentSquare + new Vector2Int(-2, 0)) == null
             && board.GetPieceOnSquare(currentSquare + new Vector2Int(-3, 0)) == null
             && board.specialMoveChecker.HasRookMoved(board.GetPieceOnSquare(currentSquare + new Vector2Int(-4, 0))) == false
            && board.specialMoveChecker.PassesCheckQueensideCastle(white) == false)
            {
                moves.Add(new Vector2Int(1, currentSquare.y));
            }
        }
        return moves;
    }

    public List<Vector2Int> StandartMoves()
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

            if (CanMoveToSquare(newPos) || CanCapture(this, newPos))
            {
                moves.Add(newPos);
            }
        }
        return moves;
    }

    public void SetHasMoved(bool moved)
    {
        hasMoved = moved;
    }
    public bool GetHasMoved()
    {
        return hasMoved;
    }
}
