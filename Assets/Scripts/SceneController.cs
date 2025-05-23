using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void RestartScene()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ToMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadDefaultGameWhite()
    {
        SceneManager.LoadScene("Default Game White");
    }

    public void LoadDefaultGameBlack()
    {
        SceneManager.LoadScene("Default Game Black");
    }

    public void QuitPlatform()
    {
        Application.Quit();
    }
}
