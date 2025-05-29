using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Nivel1"); 
    }
    public void ContinueGame()
    {

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
