using UnityEngine;
namespace GameAI { public abstract class EnemyStateBase : IState { protected readonly EnemyController enemy; protected EnemyStateBase(EnemyController e) { enemy = e; } public abstract void Enter(); public abstract void Execute(); public abstract void Exit(); } }


namespace GameAI
{
    public class PatrolState : EnemyStateBase
    {
        public PatrolState(EnemyController e) : base(e) { }
        public override void Enter() => MoveToNext();
        public override void Execute()
        {
            if (enemy.Sensor.Detect(enemy.Player)) { enemy.SwitchState(enemy.ChaseState); return; }
            if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance) MoveToNext();
        }
        public override void Exit() { }
        private void MoveToNext() => enemy.Agent.SetDestination(enemy.PatrolStrategy.GetNextDestination());
    }
}