using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    [Header("Pantalla de carga")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Image sceneImage;
    [SerializeField] Sprite nivel1, nivel2, nivel3;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Singleton enforcement
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre escenas
    }

    public void LoadSceneByName(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            StopMusic();
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"❌ Escena no encontrada: {sceneName}");
        }
    }
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StopMusic();
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"❌ Índice de escena inválido: {sceneIndex}. Verifica tu Build Settings.");
        }
    }


    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StopMusic();
            LoadSceneWithImage(nextIndex);
            //SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("🚫 No hay más escenas en el build.");
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // Asumiendo que MainMenu está primero
    }

    private void StopMusic()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }
    }
    public void LoadSceneWithImage(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingPanel.SetActive(true);
        switch (sceneIndex)
        {
            case 1:
                sceneImage.sprite = nivel1;
                break;
            case 2:
                sceneImage.sprite = nivel2;
                break;
            case 3:
                sceneImage.sprite = nivel3;
                break;

        }       

        yield return new WaitForSeconds(1f); 

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingPanel.SetActive(false);
    }
}
