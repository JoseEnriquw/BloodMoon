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

    
    [Header("Audio")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float walkClipCooldown = .7f;
    private float walkTimer;

 
    private EnemyState state = EnemyState.Patrolling;

   
    #region Inicialización 
    public void AssignPatrolData(Vector3 center, float radius, Transform[] points)
    {
        patrolCenter = center;
        patrolRadius = radius;
        patrolPoints = points;

        
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;
        col.center = transform.InverseTransformPoint(center);
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
        if (patrolPoints != null && patrolPoints.Length > 0 && agent.isOnNavMesh)
            agent.SetDestination(patrolPoints[0].position);
    }
    #endregion

    #region Update
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
    #endregion

    #region Patrullaje
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
    #endregion

    #region Triggers
    void OnTriggerStay(Collider other)
    {
        if (health.isDead || !other.CompareTag("Player")) return;
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
        if (!other.CompareTag("Player")) return;

        CancelAttack();
        state = EnemyState.Returning;
        agent.isStopped = false;
        if (agent.isOnNavMesh)
            agent.SetDestination(patrolCenter);
    }
    #endregion

    #region Ataque
    void AttackPlayer()
    {
        animator.SetTrigger("IsAttacking");
        if (hitClip) audioSrc.PlayOneShot(hitClip);
        agent.isStopped = true;       
        Invoke(nameof(ResetAttack), timeBetweenAttks);
        alreadyAttacked = true;
    }
    void ApplyDamage()            
    {
        if (player && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            HealthSystem hp = player.GetComponent<HealthSystem>();
            if (hp) hp.TakeDamage(10);
        }
    }
    void CancelAttack()
    {
        animator.ResetTrigger("IsAttacking");
        animator.CrossFade("Locomotion", 0.05f);
        alreadyAttacked = false;
        state = EnemyState.Chasing;
        agent.isStopped = false;
    }
    void ResetAttack()
    {
        alreadyAttacked = false;

        float d = Vector3.Distance(transform.position, player.position);
        state = (d <= attackRange) ? EnemyState.Attacking :
                (d <= detectionRange) ? EnemyState.Chasing :
                EnemyState.Returning;

        agent.isStopped = false;
    }
    #endregion

    #region Config
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
    #endregion
}
