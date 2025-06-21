using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyAnimator animator;

    private IEnemyState currentState;

    private void Awake()
    {
        currentState = new PatrolState(this);
    }

    private void Update()
    {
        if (combat.IsDead()) return;

        currentState?.Tick();
        combat.Tick(); // para cooldowns de ataque, etc.
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public EnemyMovement Movement => movement;
    public EnemyCombat Combat => combat;
    public EnemyAnimator Animator => animator;
}
