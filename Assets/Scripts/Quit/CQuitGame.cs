using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CQuitGame : MonoBehaviour
{
    public Button quitButton;
    public Button yesButton;
    public Button noButton;

    public GameObject popup;

    private void Awake()
    {
        popup.SetActive(false);

        quitButton.onClick.AddListener(QuitGame);
        yesButton.onClick.AddListener(YesButton);
        noButton.onClick.AddListener(NoButton);
    }

    private void QuitGame()
    {
        popup.SetActive(true);
    }

    private void YesButton()
    {
        Application.Quit();
    }

    private void NoButton()
    {
        popup.SetActive(false);
    }
}
