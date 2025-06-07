using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private HealthSystem healthSystem;
    private bool isDead = false;
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        playerData.MaxHealth = 100;

        // Setear salud inicial desde el PlayerData
        healthSystem.SetMaxHealth((int)playerData.MaxHealth);
        healthSystem.SetCurrentHealth((int)playerData.Health);

        
        healthSystem.OnHealthChanged += UpdatePlayerData;
        healthSystem.OnDeath += PlayerDeath;
    }

    private void UpdatePlayerData(int currentHealth)
    {
        // Limitar dentro de rango v�lido
        playerData.Health = Mathf.Clamp(currentHealth, 0, playerData.MaxHealth);
    }

    private void PlayerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("Player is dead");
            // Aqu� pod�s poner animaci�n, reinicio de nivel, etc.
        }
    }
}
