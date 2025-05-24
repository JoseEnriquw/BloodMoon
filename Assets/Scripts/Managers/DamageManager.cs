using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour, ICollectible
{
    [SerializeField] float _value;
    public void Collect(PlayerData playerData)
    {
        playerData.Health -= _value;
    }
    // public float value { get { return _value; } set { _value = value; } }
}
