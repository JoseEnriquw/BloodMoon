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
    private void Awake()
    {      
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {       
        currentHealth = maxHealth;
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
            animator.SetBool("IsDead ", true);
            audioSource.PlayOneShot(deathSound);
            var nav = GetComponent<NavMeshAgent>();
            if (nav != null)
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
    }  

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(3f);
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }      
       
        Destroy(gameObject);
    }


}
