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

    //BALA HACE DAÒO AL enemigo
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var healthcomponent = other.GetComponent<EnemyHealth>();
            if (healthcomponent != null)
            {
                healthcomponent.ReciveDamage(_value);
            }
        }
    }
}
