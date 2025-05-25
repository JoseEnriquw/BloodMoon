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
    [SerializeField] private string sceneToLoad = "Nivel1";
    [SerializeField] private float delay = 1.5f; // segundos opcionales de espera

    private void Start()
    {
        StartCoroutine(LoadGameAfterDelay());
    }

    private IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneToLoad);
    }
}
