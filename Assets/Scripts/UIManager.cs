using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private GameObject[] pawnPromotionPopUp;
    [SerializeField] private Sprite[] promotionPieces;
    public void ShowGameEndUI(int ending)
    {
        //endings: 1 - White victory, 2 - Black victory, 3 - Stalemate
        endScreen.SetActive(true);
        String endMessage;
        if (ending == 1)
        {
            endMessage = "Checkmate! White wins";
        }
        else if (ending == 2)
        {
            endMessage = "Checkmate! Black wins";
        }
        else
        {
            endMessage = "Stalemate!";
        }
        endText.text = endMessage;
    }

    public void ShowPawnPromotionUI(Vector2Int UIpos, bool white)
    {
        Debug.Log("pawn promotion UI");
        int W = (white == true) ? 0 : 1;
        pawnPromotionPopUp[W].SetActive(true);
        float offset = (UIpos.y == 0) ? -1.25f : 1.25f;
        pawnPromotionPopUp[W].transform.position = new Vector3(UIpos.x, UIpos.y + offset, pawnPromotionPopUp[W].transform.position.z);
    }

    public void HidePawnPromotionUI(bool white)
    {
        int W = (white == true) ? 0 : 1;
        pawnPromotionPopUp[W].SetActive(false);
    }

    public void ChangePieceSprite(Piece piece, int sprite, bool white)
    {
        int changeColor = (white == true) ? 0 : 4;
        SpriteRenderer spriteRenderer = piece.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = promotionPieces[sprite + changeColor];
        HidePawnPromotionUI(white);
    }
}
