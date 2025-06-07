using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectile; // Asigna el prefab desde el Inspector
    public float shootForce = 32f;
    public float upwardForce = 8f;
    public float timeBetweenAttacks = 1f;

    private bool alreadyAttacked = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (alreadyAttacked) return;

        // Calcular posición donde instanciar el proyectil (enfrente del jugador)
        Vector3 spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 0.5f;

        // Instanciar el proyectil
        Rigidbody rb = Instantiate(projectile, spawnPos, Quaternion.identity).GetComponent<Rigidbody>();

        // Aplicar fuerzas al proyectil
        rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        // Bloquear el ataque por un tiempo
        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
