using UnityEngine;
using UnityEngine.AI;

public class AnimalBehavior : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float maxHealth = 100f;
    float currentHealth;

    Transform player;
    NavMeshAgent agent;
    Animator animator;

    enum AnimalState { Idle, Walking, Running, Hurt, Dead }
    AnimalState currentState = AnimalState.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        // Comenzar con un destino aleatorio (si querés que patrullen)
        Wander();
    }

    void Update()
    {
        if (currentState == AnimalState.Dead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        float speed = agent.velocity.magnitude;
        animator.SetFloat("velocity", speed);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth < maxHealth * 0.5f)
        {
            SetState(AnimalState.Hurt);
        }
        else if (distanceToPlayer < detectionRadius)
        {
            FleeFromPlayer();
        }
        else
        {
            SetState(AnimalState.Walking);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Wander();
        }
    }

    void Wander()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FleeFromPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 runTo = transform.position + direction * 5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(runTo, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        SetState(AnimalState.Running);
    }

    void SetState(AnimalState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case AnimalState.Walking:
                agent.speed = 1.5f;
                break;
            case AnimalState.Hurt:
                agent.speed = 0.6f;
                break;
            case AnimalState.Running:
                agent.speed = 4f;
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    void Die()
    {
        currentState = AnimalState.Dead;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}