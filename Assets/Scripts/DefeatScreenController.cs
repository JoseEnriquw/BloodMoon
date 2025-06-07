using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatScreenController : MonoBehaviour
{
    public CanvasGroup defeatCanvas;
    public float fadeDuration = 1.5f;
    private bool fading = false;

    public void ShowDefeatScreen()
    {
        defeatCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            defeatCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        defeatCanvas.alpha = 1;
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
