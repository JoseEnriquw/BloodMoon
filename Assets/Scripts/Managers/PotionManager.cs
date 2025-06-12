using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PotionManager : MonoBehaviour, ICollectible
{
    [SerializeField] float _value;
    public void Collect(PlayerData playerData)
    {
        // playerData.Health += _value;
        if (playerData.Health >= playerData.MaxHealth)
        {
            Debug.Log("Salud ya está al máximo");
            return;
        }
        var act = playerData.Health + _value;
        if(act >playerData.MaxHealth)
        {
            playerData.Health = playerData.MaxHealth;
            return;
        }
        playerData.Health += _value;

        Debug.Log($"Cura: +{_value} HP (Total: {playerData.Health})");

    }

}
