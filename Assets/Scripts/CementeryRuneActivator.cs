using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CementeryRuneActivator : MonoBehaviour, ICollectible
{
    [SerializeField] float _value;
    private bool collected = false;
    [Tooltip("¿Destruir este objeto después de ser recogido?")]
    public bool destroyAfterPickup = true;
    public void Collect(PlayerData playerData)
    {
        collected = true;

        // Buscar el FarmGenerator y revelar la runa
        FarmGenerator farmGen = FindObjectOfType<FarmGenerator>();
        if (farmGen != null)
        {
            farmGen.RevelarRuna();
        }
        else
        {
            Debug.LogWarning("❌ No se encontró el FarmGenerator en la escena.");
        }

        // Destruir el objeto o desactivarlo
        if (destroyAfterPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);

        Debug.Log("🎁 Objeto especial recogido, runa revelada.");

    }

}

