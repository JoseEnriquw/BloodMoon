using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxHealth=100;
    [SerializeField] int currentHealth;
    //[SerializeField] private PlayerData playerData;
    //[SerializeField] Animator anim;
    public System.Action OnDeath; // Evento al morir
    public System.Action<int> OnHealthChanged; // Evento al tomar daño o curarse
    private void Start()
    {
        currentHealth=maxHealth;
        // playerData.Health=currentHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth-=amount;
        //playerData.Health = currentHealth;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(); // Notifica que murió
            //dead animation
            //anim.SetBool("IsDead", true);
        }
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth > maxHealth)
        {
            //playerData.Health = maxHealth;
            //heal animation
            //anim.SetBool("IsHeal", true);
            return;
        }
        
    }
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

}
