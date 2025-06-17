using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defetPanel;

    public static MainMenu Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // public AudioMixer audioMixer;
    public void PlayGame()
    {
        mainMenuCanvas.SetActive(false);
        GameManager.gameManager.ResetData();
        GameSceneManager.Instance.LoadSceneByIndex(1);
    }
    public void ContinueGame()
    {
        mainMenuCanvas.SetActive(false);
        GameManager.gameManager.LoadData();
    }
    public void ReintentarGame()
    {
        defetPanel.SetActive(false);
        GameManager.gameManager.LoadData();
    }
    public void OpenOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionsPanel.SetActive(true);
        
    }

    public void OpenCredits()
    {
        mainMenuCanvas.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void BacktoMenu()
    {
        mainMenuCanvas.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defetPanel.SetActive(false);    
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
    public void Victory()
    {
        mainMenuCanvas.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        victoryPanel.SetActive(false);
        //GameSceneManager.Instance.LoadSceneByIndex(0);
    }

   
}
