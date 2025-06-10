using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   // ←  ¡importante!

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ZombieHorde
    {
        public GameObject zombiePrefab;
        public int numberToSpawn = 3;
    }

    [Header("Hordas")] public List<ZombieHorde> hordes;
    [Header("Mapa")] public LayerMask groundLayer;
    [Header("Radio mapa")] public float mapRadius = 100f;   

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

                Vector3 navPos = GetRandomNavMeshPosition();


                GameObject z = Instantiate(horde.zombiePrefab, navPos, Quaternion.identity);


                var agent = z.GetComponent<NavMeshAgent>();
                if (agent && !agent.isOnNavMesh)
                    agent.Warp(navPos);

                Transform[] pts = GeneratePatrolPointsAround(navPos, /*radius*/ (int)10f, /*count*/ 4);
                z.GetComponent<EnemiesController>()
                  .AssignPatrolData(navPos, 10f, pts);
            }
        }
    }
    
    public Vector3 GetRandomNavMeshPosition()
    {
        for (int tries = 0; tries < 10; tries++)
        {
            Vector3 rnd = transform.position + Random.insideUnitSphere * mapRadius;
          
            if (Physics.Raycast(rnd + Vector3.up * 50, Vector3.down, out var hit, 100f, groundLayer))
            {
                
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 1f, NavMesh.AllAreas))
                    return nav.position;
            }
        }        
       
        return transform.position;
    }

    private Transform[] GeneratePatrolPointsAround(Vector3 center, int count, float radius)
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < count; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 pos = new Vector3(center.x + circle.x, center.y + 5, center.z + circle.y);

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10f, groundLayer))
            {
                GameObject point = new GameObject("PatrolPoint");
                point.transform.position = hit.point;
                points.Add(point.transform);
            }
        }
        return points.ToArray();
    }
}
