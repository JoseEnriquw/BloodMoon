using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletsManager : MonoBehaviour, ICollectible
{
    [SerializeField] int _value;
    public void Collect(PlayerData playerData)
    {
        playerData.Bullets += _value;
    }
   
    //BALA HACE DAÒO AL PLAYER
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            var healthcomponent=other.GetComponent<HealthSystem>();
            if(healthcomponent != null)
            {
                healthcomponent.TakeDamage(_value);
            }
        }
    }
}
