using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        healthSystem.OnDeath += EnemyDeath;
    }

    private void EnemyDeath()
    {
        Debug.Log("Enemy muerto");
       // Destroy(gameObject); // o animación de muerte
    }
}
