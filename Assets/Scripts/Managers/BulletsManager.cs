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
    //public int value { get { return _value; } set { _value = value; } }
    
}
