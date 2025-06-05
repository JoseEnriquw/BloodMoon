using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour
{
    public CanvasGroup victoryCanvas;
    public float fadeDuration = 1.5f;
    private bool fading = false;

    public Image moonImage;
    public Color redColor = Color.red;
    public Color whiteColor = Color.white;
    public float moonFadeDuration = 5f;
    private bool isRed = false;

    private void Start()
    {
        moonImage.color = redColor;
        isRed = true;
        
    }
    public void ShowVictoryScreen()
    {
        victoryCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(FadeIn());
        ToggleMoonColor();
    }
    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            victoryCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        victoryCanvas.alpha = 1;
        
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ToggleMoonColor()
    {
        moonImage.CrossFadeColor(isRed ? whiteColor : redColor, moonFadeDuration, true, true);
        isRed = !isRed;
    }
}
