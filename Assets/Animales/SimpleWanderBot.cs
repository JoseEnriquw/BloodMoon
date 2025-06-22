using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWanderBot : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;
    public float waitAfterCollision = 0.5f;

    private bool isTurning = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (!isTurning)
        {
            rb.velocity = transform.forward * moveSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isTurning)
        {
            StartCoroutine(TurnAfterCollision());
        }
    }

    private System.Collections.IEnumerator TurnAfterCollision()
    {
        isTurning = true;
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(waitAfterCollision);

        float angle = Random.Range(90f, 180f);
        float rotated = 0f;

        while (rotated < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, step, 0f);
            rotated += step;
            yield return null;
        }

        isTurning = false;
    }
}
