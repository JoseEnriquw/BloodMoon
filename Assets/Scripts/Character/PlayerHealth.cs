using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;  
    private bool isDead = false;
   // Animator anim;
    private void Awake()
    {
      
    }

    private void Start()
    {
        playerData.MaxHealth = 100;
       // anim= GetComponent<Animator>();       
    }

   
    public void ReciveHealth(int health)
    {
        SetHealth(playerData.Health + health);         
    }   
    public void ReciveDamage(int damage)
    {
        
        SetHealth(playerData.Health - damage);
        if (playerData.Health <=0 &&!isDead)
        {
            isDead = true;
            GameManager.Instance.Perder();
            // anim.SetBool("isDeath", true);
            //pasar a pantalla de derrota
            return;
        }
    }
    private void SetHealth(float value)
    {
        playerData.Health = Mathf.Clamp(value, 0, playerData.MaxHealth);
    }

}
