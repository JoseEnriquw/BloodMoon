using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float value)
    {
        animator.SetFloat("XSpeed", value);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}
