using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void PlayAgain()
    {
        BoardManager.Instance.WhiteWin.SetActive(false);
        BoardManager.Instance.BlackWin.SetActive(false);
        BoardManager.Instance.Tie.SetActive(false);
        BoardManager.Instance.StartAgain();
    }

    public void Restart()
    {
        BoardManager.Instance.WhiteWin.SetActive(false);
        BoardManager.Instance.BlackWin.SetActive(false);
        BoardManager.Instance.Tie.SetActive(false);
        BoardManager.Instance.StartAgain();
    }
}
