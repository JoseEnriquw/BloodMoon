public class ChaseState : IEnemyState
{
    private readonly EnemyController controller;

    public ChaseState(EnemyController ctrl)
    {
        controller = ctrl;
    }

    public void OnEnter()
    {
        controller.Movement.StartChase();
    }

    public void Tick()
    {
        controller.Movement.Chase(controller.Combat.GetPlayerPosition());

        if (controller.Combat.CanAttack())
            controller.ChangeState(new AttackState(controller));
    }

    public void OnExit()
    {
        controller.Movement.Stop();
    }
}
