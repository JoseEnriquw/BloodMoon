using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //SceneManager.LoadScene("Nivel1"); 
        GameManager.gameManager.ResetData();
        GameSceneManager.Instance.LoadSceneByIndex(1);
    }
    public void ContinueGame()
    {
        GameManager.gameManager.LoadData();
    }
    public void OpenOptions()
    {
        
    }

    public void OpenCredits()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
