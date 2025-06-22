
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ZombieHorde
    {
        public GameObject zombiePrefab;
        public int numberToSpawn = 3;
    }

    [Header("Hordas")]
    public List<ZombieHorde> hordes;

    [Header("Mapa")]
    public LayerMask groundLayer;

    [Header("Radio mapa")]
    public float mapRadius = 100f;

    [Header("Zona segura (padding)")]
    public float safeBorder = 2f;

    [Header("Offset altura para evitar que atraviesen el suelo")]
    public float verticalOffset = 0.5f;

    [Header("Daño de zombie")]
    public int damage=20;
    private EnemiesController enemiesController;

    public void Start()
    {
        
        //ZSpawner();
    }

    public void ZSpawner()
    {
        
        
        

        foreach (var horde in hordes)
        {
            for (int i = 0; i < horde.numberToSpawn; i++)
            {
                Vector3 spawnPos = GetSafeNavMeshPosition();
                if (spawnPos == Vector3.zero)
                {
                    Debug.LogWarning("❌ No se encontró posición válida para zombie.");
                    continue;
                }

                GameObject z = Instantiate(horde.zombiePrefab, spawnPos, Quaternion.identity);
                var agent = z.GetComponent<NavMeshAgent>();

                if (agent && !agent.isOnNavMesh)
                    agent.Warp(spawnPos);

                Transform[] patrolPoints = GeneratePatrolPointsAround(spawnPos, 4, 10f);
                z.GetComponent<EnemiesController>()?.AssignPatrolData(spawnPos, 10f, patrolPoints);
                z.GetComponent<EnemiesController>()?.ConfigDamage(damage);
            }
        }

      
    }

    private Vector3 GetSafeNavMeshPosition()
    {
        for (int tries = 0; tries < 20; tries++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-mapRadius + safeBorder, mapRadius - safeBorder),
                0,
                Random.Range(-mapRadius + safeBorder, mapRadius - safeBorder)
            );

            // Raycast hacia abajo desde arriba para encontrar el suelo
            if (Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out var hit, 100f, groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 2f, NavMesh.AllAreas))
                {
                    // Le sumamos un pequeño offset para que no atraviese el suelo
                    return nav.position + Vector3.up * verticalOffset;
                }
            }
        }

        return Vector3.zero; // fallback
    }

    private Transform[] GeneratePatrolPointsAround(Vector3 center, int count, float radius)
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector3 probe = new Vector3(center.x + offset.x, center.y + 5, center.z + offset.y);
            if (Physics.Raycast(probe, Vector3.down, out RaycastHit hit, 10f, groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
                {
                    GameObject point = new GameObject("PatrolPoint");
                    point.transform.position = navHit.position + Vector3.up * verticalOffset;
                    points.Add(point.transform);
                }
            }
        }

        return points.ToArray();
    }
}

