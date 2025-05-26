using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesController : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public GameObject projectile;

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

    private void Awake()
    {
        
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        var sc = GetComponent<SphereCollider>();
        if (sc == null)
        {
            sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = triggerRadius;
        }
    }

    private void Update()
    {
        if (!playerInTrigger && !walkPointSet)
            SearchWalkPoint();

        if (!playerInTrigger)
            Patroling();
       
    }
    private void Patroling()
    {
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = true;
        agent.isStopped = false;
        agent.SetDestination(other.transform.position);

       
        Vector3 lookDir = (other.transform.position - transform.position).normalized;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                 Quaternion.LookRotation(lookDir),
                                                 Time.deltaTime * 5f);

        
        if (HasLineOfSight(other.transform))
            AttackPlayer();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;
        agent.isStopped = true;
        alreadyAttacked = false;  
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
        if (alreadyAttacked) return;

        
        agent.isStopped = true;

       
        Vector3 spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 0.5f;
        var rb = Instantiate(projectile, spawnPos, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 8f, ForceMode.Impulse);

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
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
    //NavMeshAgent agent;


    //void Start()
    //{
    //    agent = GetComponent<NavMeshAgent>();
    //}
    ////eje z es el frente del pj, usar raicast para hacer el danio
    //// el piso tiene que tener un NavMeshSurface , bakeado.

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        agent.isStopped = false;
    //        agent.SetDestination(other.gameObject.transform.position);
    //    }

    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        agent.isStopped = true;
    //}
}
