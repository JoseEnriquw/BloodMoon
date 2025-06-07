using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour, ICollectible
{
    //ESO SIRVE PARA UNA ZONA QUE HAGA DAÒO (PISO, PICOS ETC)
    [SerializeField] float _value;
    public void Collect(PlayerData playerData)
    {
        playerData.Health -= _value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var healthcomponent = other.GetComponent<HealthSystem>();
            if (healthcomponent != null)
            {
                healthcomponent.TakeDamage((int)_value);
            }
        }
    }
}
