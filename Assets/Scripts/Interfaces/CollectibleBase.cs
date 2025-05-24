using UnityEngine;

public abstract class CollectibleBase : MonoBehaviour, ICollectible
{
    [SerializeField] protected float value;

    public float GetValue()
    {
        return value;
    }

    public abstract void Collect(PlayerData playerData);
}
