using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
   
    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarVictoria(); // O Victory(), según cómo se llame
        }
        else
        {
            Debug.LogError("❌ No se encontró el UIManager.");
        }
    }

    
}
