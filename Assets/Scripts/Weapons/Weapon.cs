using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] string WeaponName = "Default Weapon";
    [SerializeField] private AudioClip shootAudioClip;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot(Vector3 aimDir, Vector3 spawnBulletPosition)
    {
        // opcional: proyectil físico
        if (BulletPrefab)
        {
            Instantiate(BulletPrefab, spawnBulletPosition,
                                          Quaternion.LookRotation(aimDir,Vector3.up));

            if(shootAudioClip != null)
                audioSource.PlayOneShot(shootAudioClip);
        }
    }
}
