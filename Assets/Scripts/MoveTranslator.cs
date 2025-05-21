using UnityEngine;

public class MoveTranslator : MonoBehaviour
{
    private string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
    private string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8" };
    public string SquareToUci(Vector2Int square)
    {
        string let = letters[square.x];
        string num = numbers[square.y];
        return let + num;
    }

    public Vector2Int UciToSquare(string uci)
    {
        string let = uci[0].ToString();
        string num = uci[1].ToString();
        int x = System.Array.IndexOf(letters, let);
        int y = System.Array.IndexOf(numbers, num);
        return new Vector2Int(x, y);
    }
}
