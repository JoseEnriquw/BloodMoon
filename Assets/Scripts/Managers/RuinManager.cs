using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RuinManager : MonoBehaviour , ICollectible
{
    [SerializeField] int _value;
    private bool _hasAll=false;
    public static event Action OnAllRunesCollected;
    public void Collect(PlayerData playerData)
    {
        playerData.Runes += _value;
        if (playerData.Runes >= 3 && !_hasAll)
        {
            _hasAll = true;
            OnAllRunesCollected?.Invoke();
        }
            
    }
     public bool HasAll() { return _hasAll; }
  
   
   
}
