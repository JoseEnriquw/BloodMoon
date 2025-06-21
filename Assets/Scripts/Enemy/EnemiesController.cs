
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrolling, Chasing, Attacking, Returning }

[RequireComponent(typeof(NavMeshAgent), typeof(SphereCollider))]
public class EnemiesController : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSrc;
    private EnemyHealth health;
    private Transform player;

    private Transform[] patrolPoints;
    private int patrolIndex;
    private Vector3 patrolCenter;
    private float patrolRadius = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float timeBetweenAttks = 1f;
    private bool alreadyAttacked;
    [SerializeField] private int attackDamage = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float walkClipCooldown = .7f;
    private float walkTimer;

    private EnemyState state = EnemyState.Patrolling;

    [SerializeField] private float speed = 1.8f;
    [SerializeField] private float acceleration = 4f;

    [Header("Especial")]
    public bool esEspecial = false;
    public void AssignPatrolData(Vector3 center, float radius, Transform[] points)
    {
        patrolCenter = center;
        patrolRadius = radius;
        patrolPoints = points;

        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;
        col.center = transform.InverseTransformPoint(center);

        if (agent != null && agent.enabled)
        {
            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(transform.position, out var navHit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(navHit.position);
                    Debug.Log($"[{name}] Warpeado a NavMesh en: {navHit.position}");
                }
                else
                {
                    Debug.LogError($"[{name}] No se pudo warpear al NavMesh.");
                    return;
                }
            }

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                state = EnemyState.Patrolling;
                patrolIndex = 0;
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[patrolIndex].position);

                if (!agent.hasPath)
                    Debug.LogWarning($"[{name}] ¡NO tiene camino! ¿Punto desconectado?");
                else
                    Debug.Log($"[{name}] Patrullando hacia: {patrolPoints[0].position}");
            }
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        health = GetComponent<EnemyHealth>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    void Start()
    {
        agent.speed = speed;
        agent.acceleration = acceleration;

        Debug.Log($"[{name}] Estado inicial: {state}, OnNavMesh: {agent.isOnNavMesh}");

        if (patrolPoints != null && patrolPoints.Length > 0 && agent.isOnNavMesh)
        {
            agent.SetDestination(patrolPoints[0].position);
           
        }
        else
        {
            Debug.LogWarning($"[{name}] No hay puntos de patrulla o no está sobre NavMesh");
        }
    }

    void Update()
    {
        if (health.isDead) return;

        switch (state)
        {
            case EnemyState.Patrolling: HandlePatrol(); break;
            case EnemyState.Chasing: animator.SetFloat("XSpeed", 3); break;
            case EnemyState.Returning:
                animator.SetFloat("XSpeed", 1);
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + .1f)
                    ToPatrol();
                break;
        }

        walkTimer -= Time.deltaTime;
    }

    void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + .1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            if (agent.isOnNavMesh)
                agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        animator.SetFloat("XSpeed", 1);
        if (!audioSrc.isPlaying)
        {
            audioSrc.PlayOneShot(walkClip);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (health.isDead || !other.CompareTag("Player")) return;

        agent.speed = 4f;
        float distToCenter = Vector3.Distance(other.transform.position, patrolCenter);
        if (distToCenter > patrolRadius)
        {
            OnTriggerExit(other);
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (state != EnemyState.Attacking && dist > attackRange)
        {
            state = EnemyState.Chasing;
            agent.isStopped = false;
            if (agent.isOnNavMesh)
                agent.SetDestination(player.position);
        }
        else if (dist <= attackRange && !alreadyAttacked)
        {
            state = EnemyState.Attacking;
            agent.isStopped = true;
            FaceTarget(player.position);
            AttackPlayer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (health.isDead) return;
        if (!other.CompareTag("Player")) return;

        CancelAttack();
        agent.speed = speed;
        state = EnemyState.Returning;
        agent.isStopped = false;
        if (agent.isOnNavMesh)
            agent.SetDestination(patrolCenter);
    }

    void AttackPlayer()
    {
        animator.SetTrigger("IsAttacking");
        if (hitClip) audioSrc.PlayOneShot(hitClip);
        agent.isStopped = true;
        Invoke(nameof(ResetAttack), timeBetweenAttks);
        alreadyAttacked = true;
    }

    void CancelAttack()
    {
        if (health.isDead) return;
        animator.ResetTrigger("IsAttacking");
        animator.CrossFade("Locomotion", 0.05f);
        alreadyAttacked = false;
        state = EnemyState.Chasing;
        agent.isStopped = false;
    }

    void ResetAttack()
    {
        if (health.isDead) return;
        alreadyAttacked = false;

        float d = Vector3.Distance(transform.position, player.position);
        state = (d <= attackRange) ? EnemyState.Attacking :
                (d <= detectionRange) ? EnemyState.Chasing :
                EnemyState.Returning;

        agent.isStopped = false;
    }

    void FaceTarget(Vector3 tgt)
    {
        Vector3 dir = (tgt - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
    }

    void ToPatrol()
    {
        state = EnemyState.Patrolling;
        GoToNearestPatrolPoint();
    }

    void GoToNearestPatrolPoint()
    {
        if (!agent.isOnNavMesh || patrolPoints == null || patrolPoints.Length == 0) return;
        float best = float.MaxValue;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < best) { best = d; patrolIndex = i; }
        }
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            patrolCenter == Vector3.zero ? transform.position : patrolCenter,
            patrolRadius);
    }

    void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.magenta;
            var corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
    void ApplyDamage()
    {

        if (player && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            var hp = player.GetComponent<PlayerHealth>();
            if (hp) hp.ReciveDamage(attackDamage);
        }
    }
    public void OnMuere()
    {
        if (esEspecial)
        {
            Debug.Log("☠️ Zombie especial eliminado. Revelando runa...");
            var generator = FindObjectOfType<FarmGenerator>();
            if (generator != null)
            {
                generator.RevelarRuna();
            }
        }
    }


}

