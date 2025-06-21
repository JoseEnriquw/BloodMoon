using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private int damage = 10;

    private float timer;
    private Transform player;
    private EnemyHealth health;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<EnemyHealth>();
    }

    public void Tick()
    {
        timer -= Time.deltaTime;
    }

    public void Attack()
    {
        FacePlayer();
        GetComponent<Animator>().SetTrigger("Attack");

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp && Vector3.Distance(transform.position, player.position) <= attackRange)
            hp.ReciveDamage(damage);

        timer = cooldown;
    }

    public bool CanSeePlayer()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    public bool CanAttack()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange && timer <= 0;
    }

    public bool IsDead()
    {
        return health != null && health.isDead;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.position;
    }

    private void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
    }
}
