using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthUI : MonoBehaviour
{
    public Slider slider;
    public Vector3 offset = new Vector3(0, 2.2f, 0); // altura sobre la cabeza
    private Transform target; // el zombie al que sigue

    public Canvas canvas;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void UpdateHealth(float current, float max)
    {
        if (slider != null)
            slider.value = current / max;
        if (canvas != null && !canvas.enabled)
            canvas.enabled = true;
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position + offset;
            transform.forward = Camera.main.transform.forward; // Siempre mirar a la cámara
        }
    }
    private void Awake()
    {
        if (canvas != null)
            canvas.enabled = false; // ocultar al principio
    }
}
