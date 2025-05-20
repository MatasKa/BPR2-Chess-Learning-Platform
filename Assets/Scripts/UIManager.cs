using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerUI;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private GameObject[] pawnPromotionPopUp;
    [SerializeField] private Sprite[] promotionPieces;

    void Update()
    {
        if (timerUI != null && endScreen.activeSelf == false)
        {
            UpdateTimerUI(timer.GetTimeMinutes(), timer.GetTimeSeconds());
        }
    }
    public void ShowGameEndUI(int ending)
    {
        String endMessage;
        if (endScreen.activeSelf == false)
        {
            if (ending == 1)
            {
                endMessage = "Checkmate! White wins";
            }
            else if (ending == 2)
            {
                endMessage = "Checkmate! Black wins";
            }
            else if (ending == 3)
            {
                endMessage = "Stalemate!";
            }
            else
            {
                endMessage = "You ran out of time!";
            }
            endText.text = endMessage;
            endScreen.SetActive(true);
        }
    }

    public void ShowPawnPromotionUI(Vector2Int UIpos, bool white)
    {
        int W = (white == true) ? 0 : 1;
        pawnPromotionPopUp[W].SetActive(true);
        float offset = (UIpos.y == 1) ? -2.35f : 2.35f;
        Debug.Log("Y pos yr " + UIpos.y + " therefore offset is " + offset);
        pawnPromotionPopUp[W].transform.position = new Vector3(UIpos.x, UIpos.y + offset, pawnPromotionPopUp[W].transform.position.z);
    }

    public void HidePawnPromotionUI(bool white)
    {
        int W = (white == true) ? 0 : 1;
        pawnPromotionPopUp[W].SetActive(false);
    }

    public void ChangePieceSprite(GameObject piece, int sprite, bool white)
    {
        int changeColor = (white == true) ? 0 : 4;
        SpriteRenderer spriteRenderer = piece.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = promotionPieces[sprite + changeColor];
        HidePawnPromotionUI(white);
    }

    private void UpdateTimerUI(int min, int sec)
    {
        timerUI.text = string.Format("{0:00}:{1:00}", min, sec);
        if (min < 1 && sec <= 30)
        {
            UpdateTimerColor(Color.red);
        }
    }
    private void UpdateTimerColor(Color color)
    {
        timerUI.color = color;
    }
}
