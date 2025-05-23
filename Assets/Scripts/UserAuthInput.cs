using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UserAuthInput : MonoBehaviour
{
    [SerializeField] private DatabaseManager db;
    [SerializeField] private TMP_InputField regUsernameInput;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;

    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private GameObject ErrorText;
    [SerializeField] private GameObject LoginMenu;
    [SerializeField] private GameObject RegisterMenu;
    public void OnLoginPress()
    {
        if (loginUsernameInput.text == "" || loginPasswordInput.text == "")
        {
            ShowErrorMessage("All fields must be filled");
        }
        else
        {
            var user = db.Login(loginUsernameInput.text, loginPasswordInput.text);
            if (user != null)
            {
                SceneManager.LoadScene("Main Menu"); // not in scene controller because rushed (would do a User class, dontDestroy between scenes...)
            }
            else
            {
                ShowErrorMessage("Incorrect username or password");
            }
        }
    }

    public void OnRegisterPress()
    {
        if (regUsernameInput.text == "" || regPasswordInput.text == "" || regEmailInput.text == "")
        {
            ShowErrorMessage("All fields must be filled");
        }
        else
        {
            db.CreateUser(regUsernameInput.text, regEmailInput.text, regPasswordInput.text);
            db.LogAllUsers();
            LoginMenu.SetActive(true);
            RegisterMenu.SetActive(false);
            ErrorText.SetActive(false);
        }
    }

    public void ShowErrorMessage(string errorMessage)
    {
        ErrorText.gameObject.SetActive(true);
        ErrorText.GetComponent<TextMeshProUGUI>().text = errorMessage;
    }
}
