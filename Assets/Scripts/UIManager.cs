using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endText;

    public void GameEndUI(int ending)
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
}
