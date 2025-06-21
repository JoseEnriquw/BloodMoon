using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    private NavMeshAgent agent;
    private int patrolIndex;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StartPatrol()
    {
        agent.isStopped = false;
        agent.speed = 1.8f;
        GoToNextPoint();
    }

    public void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            GoToNextPoint();
        }
    }

    public void StartChase()
    {
        agent.speed = 3.5f;
        agent.isStopped = false;
    }

    public void Chase(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    private void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }
}
