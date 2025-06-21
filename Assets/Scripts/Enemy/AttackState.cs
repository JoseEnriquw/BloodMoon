public class AttackState : IEnemyState
{
    private readonly EnemyController controller;

    public AttackState(EnemyController ctrl)
    {
        controller = ctrl;
    }

    public void OnEnter()
    {
        controller.Combat.Attack();
    }

    public void Tick()
    {
        if (!controller.Combat.CanAttack())
        {
            controller.ChangeState(new ChaseState(controller));
        }
    }

    public void OnExit() { }
}
