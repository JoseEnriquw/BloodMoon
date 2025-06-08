using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesController2 : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public GameObject projectile;
    EnemyHealth helth;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    [Header("Patrol Settings")]
    public float patrolPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet = false;
    public float walkPointRange;

    [Header("Detection/Trigger")]
    public float triggerRadius = 8f;
    private bool playerInTrigger = false;

    [Header("Attack Settings")]
    public float attackRayDistance = 4f;
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked = false;

    private Transform playerTransform;
    Animator animator;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Sounds Settings")]
    [SerializeField] private AudioClip walksound;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    private void Awake()
    {

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        helth = GetComponent<EnemyHealth>();
        var sc = GetComponent<SphereCollider>();
        if (sc == null)
        {
            sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = triggerRadius;
        }
        agent.stoppingDistance = 2.3f;
    }

    private void Update()
    {
        if (helth.isDead) return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;


        if (!playerInTrigger && !walkPointSet)
            // SearchWalkPoint();

            if (!playerInTrigger)
                Patroling();

    }
    private void Patroling()
    {
        if (patrolPoints.Length == 0 || agent.pathPending) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        // Debug log opcional
        // Debug.Log($"Yendo a punto {currentPatrolIndex}, distancia restante: {agent.remainingDistance}");

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            // Debug.Log("Llegó al punto, cambiando al siguiente.");
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        //float speed = agent.velocity.magnitude; no sirve xq va cambiando constantemente
        animator.SetFloat("XSpeed", 1);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(walksound);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (helth.isDead || !other.CompareTag("Player")) return;

        playerInTrigger = true;


        if (animator.GetBool("IsAttacking")) return;

        float distanceToPlayer = Vector3.Distance(transform.position, other.transform.position);

        if (distanceToPlayer > agent.stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(other.transform.position);
        }
        else
        {
            agent.isStopped = true;

            Vector3 lookDir = (other.transform.position - transform.position).normalized;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                     Quaternion.LookRotation(lookDir),
                                                     Time.deltaTime * 5f);

            if (HasLineOfSight(other.transform))
                AttackPlayer();
        }

        animator.SetFloat("XSpeed", 3);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;
        agent.isStopped = false;// true;
        alreadyAttacked = false;
        animator.SetFloat("XSpeed", 1);
        ResetPatrolToNearestPoint();
    }
    private void ResetPatrolToNearestPoint()
    {
        float minDistance = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
        animator.SetFloat("XSpeed", 1);
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target.position - origin).normalized;
        float dist = Vector3.Distance(origin, target.position);

        if (dist > attackRayDistance) return false;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, attackRayDistance, playerLayer | groundLayer))
        {
            Debug.DrawRay(origin, dir * attackRayDistance, Color.red);
            return hit.transform == target;
        }
        return false;
    }
    private void AttackPlayer()
    {
        audioSource.Stop();
        if (alreadyAttacked) return;

        agent.ResetPath();
        agent.isStopped = true;

        animator.SetTrigger("IsAttacking");
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(hitSound);
        }

        alreadyAttacked = true;

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        if (helth.isDead) return;

        alreadyAttacked = false;

        if (playerInTrigger)
            agent.isStopped = false;
    }


    public void TakeDamage(float damage)
    {

    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);


        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = playerTransform != null
            ? (playerTransform.position - origin).normalized
            : transform.forward;
        Gizmos.DrawLine(origin, origin + dir * attackRayDistance);
    }

    public void ApplyDamage()
    {
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < 2f)
        {
            HealthSystem playerHealth = playerTransform.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }
    }

}
