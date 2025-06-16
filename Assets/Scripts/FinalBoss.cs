using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBoss : MonoBehaviour
{
    [SerializeField] private GameObject portal;
     private ComicSequence comicSequence;
    public static event Action portalActivated;
    private void Start()
    {
        portal.SetActive(false);
        RuinManager.OnAllRunesCollected += ActivatePortal;
        
    }

    private void OnDestroy()
    {
        RuinManager.OnAllRunesCollected -= ActivatePortal; 
    }

    private void ActivatePortal()
    {
        if (!portal.activeSelf)
        {
            portal.SetActive(true);
            Debug.Log("🚪 Portal activado al recolectar 3 runas!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (portal.activeSelf && other.CompareTag("Player"))
        {
            Debug.Log("🎬 Entrando al portal. Iniciando historia final...");
            if (ComicSequence.Instance != null)
            {
                ComicSequence.Instance.StartComicSequence("final");
            }
            else
            {
                Debug.LogError("❌ ComicSequence.Instance es null");
            }

            //portalActivated?.Invoke();
            //comicSequence = FindObjectOfType<ComicSequence>();
            //comicSequence.StartComicSequence("final");
            this.enabled = false; // Evita múltiples disparos
        }
    }
}
