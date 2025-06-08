using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject BulletPrefab;
    public float BulletRange = 100f;
    public float BulletSpeed = 20f;

    void Start()
    {
    }

    void Update()
    {
    }

    public void Shoot()
    {
        // raycast instant�neo
        Ray ray = new(gameObject.transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, BulletRange))
        {
            // impacto: aplicar da�o, efectos, etc.
        }
        // opcional: proyectil f�sico
        if (BulletPrefab)
        {
            GameObject proj = Instantiate(BulletPrefab, gameObject.transform.position,
                                          Quaternion.LookRotation(transform.forward));
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.velocity = transform.forward * BulletSpeed;
        }
    }
}
