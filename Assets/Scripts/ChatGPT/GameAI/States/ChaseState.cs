using UnityEngine;
namespace GameAI
{
    public class ChaseState : EnemyStateBase
    {
        public ChaseState(EnemyController e) : base(e) { }
        public override void Enter()
        {
            enemy.Agent.speed = enemy.chaseSpeed;
            enemy.LastKnownPosition = enemy.Player.position;
            enemy.Agent.SetDestination(enemy.LastKnownPosition);
        }
        public override void Execute()
        {
            if (enemy.Sensor.Detect(enemy.Player))
            {
                enemy.LastKnownPosition = enemy.Player.position;
                enemy.Agent.SetDestination(enemy.LastKnownPosition);
                return;
            }
            enemy.SwitchState(enemy.SearchState);
        }
        public override void Exit() { }
    }
}