using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyHealth : MonoBehaviour
{
    
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth;
    Animator animator;
    public bool isDead = false;
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    [SerializeField] private GameObject healthBarPrefab;
    private ZombieHealthUI healthBarUI;
    private void Awake()
    {      
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {       
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            healthBarUI = bar.GetComponent<ZombieHealthUI>();
            if (healthBarUI != null)
                healthBarUI.SetTarget(transform);
        }
    }
   
    public void ReciveHealth(int health)
    {
        SetHealth(currentHealth + health);
    }
    public void ReciveDamage(int damage)
    {
        SetHealth(currentHealth - damage);        
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.ResetTrigger("IsAttacking");
            animator.SetBool("IsDead ", true);
            audioSource.PlayOneShot(deathSound);
            var controller = GetComponent<EnemiesController>();
            if (controller != null)
            {
                controller.OnMuere();
            }
            if (TryGetComponent<NavMeshAgent>(out var nav))
            {
                nav.isStopped = true;
                nav.enabled = false;
            }
            StartCoroutine(DestroyAfterDeath());          
            return;
        }
    }
    private void SetHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);

        if (healthBarUI != null)
            healthBarUI.UpdateHealth(currentHealth, maxHealth);
    }  

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(3f);
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }
        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        Destroy(gameObject);


    }


}
