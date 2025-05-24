using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private readonly List<ICollectible> nearbyCollectibles = new List<ICollectible>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCollectAll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICollectible>(out var collectible))
        {
            nearbyCollectibles.Add(collectible);

            // Si es daño, aplicar instantáneamente
            if (collectible is DamageManager)
            {
                collectible.Collect(playerData);
                nearbyCollectibles.Remove(collectible);
                // Opcional: Destroy(((MonoBehaviour)collectible).gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICollectible>(out var collectible))
        {
            nearbyCollectibles.Remove(collectible);
        }
    }

    private void TryCollectAll()
    {
        foreach (var collectible in new List<ICollectible>(nearbyCollectibles))
        {
            collectible.Collect(playerData);
            Destroy(((MonoBehaviour)collectible).gameObject);
            nearbyCollectibles.Remove(collectible);
        }
    }
    

}
