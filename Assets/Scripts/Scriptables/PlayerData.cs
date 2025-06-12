using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerData", menuName ="CreatePlayerData", order =0)]
public class PlayerData : ScriptableObject
{
    public float Health;
    public float MaxHealth;
    public int Bullets;
    public int Runes;
    public Vector3 LastPosition;
}
