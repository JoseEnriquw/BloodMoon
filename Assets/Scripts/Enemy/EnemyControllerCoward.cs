using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(SphereCollider))]
public class EnemyControllerCoward : MonoBehaviour
{
    [Header("Componentes")]
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSrc;
    private Transform player;
    private EnemyHealth health;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    private int patrolIndex;
    private float patrolRadius = 10f;

    [Header("Fuga")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float escapeDistance = 12f;
    [SerializeField] private float escapeSpeed = 4f;

    private Vector3 patrolCenter;
    private bool isFleeing = false;
    private float normalSpeed;

    [Header("Audio")]
    [SerializeField] private AudioClip walkClip;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<EnemyHealth>();
        normalSpeed = agent.speed;
    }

    void Start()
    {
        patrolCenter = transform.position;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (health != null && health.isDead) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= detectionRange)
        {
            FleeFromPlayer();
        }
        else if (isFleeing && distToPlayer > escapeDistance)
        {
            StopFleeing();
        }
        else if (!isFleeing)
        {
            Patrol();
        }

        animator.SetFloat("XSpeed", agent.velocity.magnitude);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            GoToNextPatrolPoint();
        }

        PlayWalkSound();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    void FleeFromPlayer()
    {
        isFleeing = true;
        agent.speed = escapeSpeed;

        Vector3 fleeDir = (transform.position - player.position).normalized;
        Vector3 targetPos = transform.position + fleeDir * 10f;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró lugar para huir.");
        }
    }

    void StopFleeing()
    {
        isFleeing = false;
        agent.speed = normalSpeed;
        GoToNextPatrolPoint();
    }

    void PlayWalkSound()
    {
        if (walkClip != null && !audioSrc.isPlaying)
        {
            audioSrc.PlayOneShot(walkClip);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
