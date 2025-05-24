using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    BulletsManager bulletsManager;   
    PotionManager potionManager;
    RuinManager ruinManager;
    [SerializeField]PlayerData playerData;

    private BulletsManager nearbyBullet;
    private PotionManager nearbyPotion;
    private RuinManager nearbyRuin;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCollect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            nearbyBullet = other.GetComponent<BulletsManager>();
        }
        else if (other.CompareTag("Potion"))
        {
            nearbyPotion = other.GetComponent<PotionManager>();
        }
        else if (other.CompareTag("Ruin"))
        {
            nearbyRuin = other.GetComponent<RuinManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (nearbyBullet == other.GetComponent<BulletsManager>())
                nearbyBullet = null;
        }
        else if (other.CompareTag("Potion"))
        {
            if (nearbyPotion == other.GetComponent<PotionManager>())
                nearbyPotion = null;
        }
        else if (other.CompareTag("Ruin"))
        {
            if (nearbyRuin == other.GetComponent<RuinManager>())
                nearbyRuin = null;
        }
    }

    private void TryCollect()
    {
        if (nearbyBullet != null)
        {
            playerData.Bullets += nearbyBullet.value;
            Destroy(nearbyBullet.gameObject);
            nearbyBullet = null;
        }

        if (nearbyPotion != null)
        {
            playerData.Health += nearbyPotion.value;
            Destroy(nearbyPotion.gameObject);
            nearbyPotion = null;
        }

        if (nearbyRuin != null)
        {
            playerData.Runes += nearbyRuin.value;
            Destroy(nearbyRuin.gameObject);
            nearbyRuin = null;
        }
    }
    
}
