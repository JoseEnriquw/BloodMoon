using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] float BulletRange = 100f;
    [SerializeField] float BulletSpeed = 20f;
    [SerializeField] string WeaponName = "Default Weapon";
    [SerializeField] Transform WeaponMuzzle;
    [SerializeField] private AudioClip shootAudioClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
    }

    public void Shoot()
    {
        // opcional: proyectil físico
        if (BulletPrefab)
        {
            GameObject proj = Instantiate(BulletPrefab, WeaponMuzzle.transform.position,
                                          Quaternion.LookRotation(WeaponMuzzle.transform.right));
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.velocity = WeaponMuzzle.transform.forward * BulletSpeed;

            if(shootAudioClip != null)
                audioSource.PlayOneShot(shootAudioClip);
        }
    }
}
