//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;   

//public class ZombieSpawner : MonoBehaviour
//{
//    [System.Serializable]
//    public class ZombieHorde
//    {
//        public GameObject zombiePrefab;
//        public int numberToSpawn = 3;
//    }

//    [Header("Hordas")] public List<ZombieHorde> hordes;
//    [Header("Mapa")] public LayerMask groundLayer;
//    [Header("Radio mapa")] public float mapRadius = 100f;

//    public void Start()
//    {
//        //ZSpawner();
//    }

//    public void ZSpawner()
//    {
//        foreach (var horde in hordes)
//        {
//            for (int i = 0; i < horde.numberToSpawn; i++)
//            {

//                Vector3 navPos = GetRandomNavMeshPosition();


//                GameObject z = Instantiate(horde.zombiePrefab, navPos, Quaternion.identity);


//                var agent = z.GetComponent<NavMeshAgent>();
//                if (agent && !agent.isOnNavMesh)
//                    agent.Warp(navPos);

//                Transform[] pts = GeneratePatrolPointsAround(navPos, /*radius*/ (int)10f, /*count*/ 4);
//                z.GetComponent<EnemiesController>()
//                  .AssignPatrolData(navPos, 10f, pts);
//            }
//        }
//    }

//    public Vector3 GetRandomNavMeshPosition()
//    {
//        for (int tries = 0; tries < 10; tries++)
//        {
//            Vector3 rnd = transform.position + Random.insideUnitSphere * mapRadius;

//            if (Physics.Raycast(rnd + Vector3.up * 50, Vector3.down, out var hit, 100f, groundLayer))
//            {

//                if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 1f, NavMesh.AllAreas))
//                    return nav.position;
//            }
//        }

//        return transform.position;
//    }

//    private Transform[] GeneratePatrolPointsAround(Vector3 center, int count, float radius)
//    {
//        List<Transform> points = new List<Transform>();
//        for (int i = 0; i < count; i++)
//        {
//            Vector2 circle = Random.insideUnitCircle * radius;
//            Vector3 pos = new Vector3(center.x + circle.x, center.y + 5, center.z + circle.y);

//            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10f, groundLayer))
//            {
//                GameObject point = new GameObject("PatrolPoint");
//                point.transform.position = hit.point;
//                points.Add(point.transform);
//            }
//        }
//        return points.ToArray();
//    }
//}
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

    private void Start()
    {
        ZSpawner();
    }
    public void ZSpawner()
    {
        foreach (var horde in hordes)
        {
            for (int i = 0; i < horde.numberToSpawn; i++)
            {
                // Obtener posición segura sobre el NavMesh
                Vector3 spawnPos = GetSafeNavMeshPosition();
                if (spawnPos == Vector3.zero)
                {
                    Debug.LogWarning("No se encontró posición válida para el zombie.");
                    continue;
                }

                // Instanciar zombie
                GameObject z = Instantiate(horde.zombiePrefab, spawnPos, Quaternion.identity);
                var agent = z.GetComponent<NavMeshAgent>();

                // Warp si no está sobre NavMesh
                if (agent && !agent.isOnNavMesh)
                {
                    agent.Warp(spawnPos);
                    Debug.Log($"Zombie warpeado a {spawnPos}");
                }

                // Generar puntos de patrulla
                Transform[] patrolPoints = GeneratePatrolPointsAround(spawnPos, 4, 8f);
                z.GetComponent<EnemiesController>()?.AssignPatrolData(spawnPos, 10f, patrolPoints);

                // 🔍 Prueba de path real
                if (agent != null)
                {
                    if (patrolPoints.Length > 0 && agent.isOnNavMesh)
                    {
                        agent.SetDestination(patrolPoints[0].position);

                        if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
                        {
                            Debug.LogWarning($"[{z.name}] no tiene path hacia {patrolPoints[0].position}");
                        }
                        else
                        {
                            Debug.Log($"[{z.name}] patrulla hacia {patrolPoints[0].position}");
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetSafeNavMeshPosition()
    {
        for (int tries = 0; tries < 10; tries++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * mapRadius;
            if (Physics.Raycast(randomPos + Vector3.up * 50, Vector3.down, out var hit, 100f, groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 2f, NavMesh.AllAreas))
                {
                    return nav.position;
                }
            }
        }

        return Vector3.zero;
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
                    point.transform.position = navHit.position;
                    points.Add(point.transform);
                }
                else
                {
                    Debug.LogWarning($"No se pudo samplear NavMesh desde {hit.point}");
                }
            }
        }

        return points.ToArray();
    }
}
