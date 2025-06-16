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
    private ComicSequence comicSequence;
    public static event Action Inicio;
    private void Start()
    {
        
        Inicio?.Invoke();
        comicSequence = FindObjectOfType<ComicSequence>();
        comicSequence.StartComicSequence("inicio");
    }

   
}
