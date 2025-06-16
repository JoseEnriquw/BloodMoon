using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class InitLoader : MonoBehaviour
{
    //[SerializeField] private string sceneToLoad = "Granja";
  /*  [SerializeField] private float delay = 1.5f;*/ // segundos opcionales de espera
    private ComicSequence comicSequence;
    public static event Action Inicio;
    private void Start()
    {
        //StartCoroutine(LoadGameAfterDelay());
        Inicio?.Invoke();
        comicSequence = FindObjectOfType<ComicSequence>();
        comicSequence.StartComicSequence("inicio");
    }

    //private IEnumerator LoadGameAfterDelay()
    //{
    //    yield return new WaitForSeconds(delay);
    //    Inicio?.Invoke();
    //    comicSequence = FindObjectOfType<ComicSequence>();
    //    comicSequence.StartComicSequence("inicio");
    //    //GameSceneManager.Instance.LoadNextScene();
    //    //SceneManager.LoadScene(sceneToLoad);
    //}
}
