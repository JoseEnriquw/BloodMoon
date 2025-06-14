using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RuinManager : MonoBehaviour , ICollectible
{
    [SerializeField] int _value;
    
    public void Collect(PlayerData playerData)
    {
        playerData.Runes += _value;
        
    }
   // public int value { get { return _value; } set { _value = value; } }
   
   
}
