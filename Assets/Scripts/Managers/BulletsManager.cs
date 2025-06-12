using UnityEngine;

public class BulletsManager : MonoBehaviour, ICollectible
{
    [SerializeField] int _value;
    [SerializeField] int _damage;
    public void Collect(PlayerData playerData)
    {
        playerData.Bullets += _value;
    }

    //BALA HACE DA�O AL enemigo
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyHealth>(out var healthcomponent))
            {
                healthcomponent.ReciveDamage(_damage);
            }
        }
    }
}
