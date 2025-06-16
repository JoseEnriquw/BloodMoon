using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ComicSequence : MonoBehaviour
{
    [SerializeField] private Canvas canvasRoot;
    [SerializeField] private Image comicImage;
    [Header("Secuencias")]
    public List<Sprite> imageInicio = new List<Sprite>();
    public List<Sprite> imageFinal = new List<Sprite>();
    // public string[] imageFinal = { "Comic/Final/comic_1", "Comic/Final/comic_2", "Comic/Final/comic_3" };
    // public string[] imageInicio = { "Comic/Inicio/inicio_1", "Comic/Final/inicio_2", "Comic/Final/inicio_3", "Comic/Final/inicio_4", "Comic/Final/inicio_5" };
    public float imageDuration = 4f;
    
    public static ComicSequence Instance { get; private set; }

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
    private void Start()
    {
        comicImage.gameObject.SetActive(false); // Oculto al inicio
        FinalBoss.portalActivated += Activate;
        InitLoader.Inicio += Activate;
    }
    private void OnDestroy()
    {
        FinalBoss.portalActivated -= Activate;
        InitLoader.Inicio -= Activate;
    }
    private void Activate()
    {
        if(!canvasRoot.gameObject.activeSelf)
            canvasRoot.gameObject.SetActive (true);
        if (!comicImage.gameObject.activeSelf)
        {
            comicImage.gameObject.SetActive(true);
            
        } 
    }
    public void StartComicSequence(string secuence)
    {
        switch (secuence.ToLower())
        {
            case "inicio":
                PlaySequence(imageInicio, 2);
                break;
            case "final":
                PlaySequence(imageFinal,3);
                break;
            default:
                Debug.LogWarning($"⚠️ No existe la secuencia '{secuence}'");
                break;
        }

    }
    public void PlaySequence(List<Sprite> sequencen, int scene )
    {
        if (comicImage != null && canvasRoot != null)
            StartCoroutine(PlayComicCoroutine(sequencen, scene));
        else
            Debug.LogError("❌ Falta asignar comicImage o canvasRoot.");
    }
   
    private IEnumerator PlayComicCoroutine(List<Sprite> sequence, int scene)
    {
        
        canvasRoot.gameObject.SetActive(true);
        comicImage.gameObject.SetActive(true);

        foreach (Sprite sprite in sequence)
        {
            comicImage.sprite = sprite;
            yield return new WaitForSeconds(imageDuration);
        }

        comicImage.gameObject.SetActive(false);
        canvasRoot.gameObject.SetActive(false);
        // GameSceneManager.Instance.LoadSceneByIndex(scene);

        GameSceneManager.Instance.LoadNextScene();
    }
   
}
