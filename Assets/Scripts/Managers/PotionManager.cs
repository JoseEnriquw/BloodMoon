using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PotionManager : MonoBehaviour, ICollectible
{
    [SerializeField] float _value;
    public void Collect(PlayerData playerData)
    {
        playerData.Health += _value;
    }
    //public float value { get { return _value; } set { _value = value; } }

}
