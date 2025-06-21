public class PatrolState : IEnemyState
{
    private readonly EnemyController controller;

    public PatrolState(EnemyController ctrl)
    {
        controller = ctrl;
    }

    public void OnEnter()
    {
        controller.Movement.StartPatrol();
    }

    public void Tick()
    {
        controller.Movement.Patrol();

        if (controller.Combat.CanSeePlayer())
            controller.ChangeState(new ChaseState(controller));
    }

    public void OnExit()
    {
        controller.Movement.Stop();
    }
}
