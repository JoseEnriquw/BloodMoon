using UnityEngine;
using UnityEngine.AI;

public class AnimalFarmSpawner : MonoBehaviour
{
    [System.Serializable]
    public class AnimalEntry
    {
        public GameObject prefab;
        public int cantidad;
    }

    public AnimalEntry[] animales;
    [Tooltip("Distancia máxima alrededor de la granja donde se spawnean")]
    public float radioSpawn = 10f;

    void OnEnable()
    {
        NavMeshEvents.OnNavMeshReady += SpawnNow;
    }

    void OnDisable()
    {
        NavMeshEvents.OnNavMeshReady -= SpawnNow;
    }

    public void SpawnNow()
    {
        foreach (var entry in animales)
        {
            for (int i = 0; i < entry.cantidad; i++)
            {
                Vector3 spawnPos = GetNearbyNavMeshPosition();
                if (spawnPos != Vector3.zero)
                {
                    Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }

    Vector3 GetNearbyNavMeshPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * radioSpawn;
            randomOffset.y = 0;
            Vector3 candidate = transform.position + randomOffset;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}