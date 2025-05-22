using UnityEngine;
namespace GameAI
{
    public class SearchState : EnemyStateBase
    {
        private float _timer; private const float SEARCH_TIME = 4f;
        public SearchState(EnemyController e) : base(e) { }
        public override void Enter()
        {
            _timer = 0f; enemy.Agent.speed = enemy.patrolSpeed; enemy.Agent.SetDestination(enemy.LastKnownPosition);
        }
        public override void Execute()
        {
            _timer += Time.deltaTime;
            if (enemy.Sensor.Detect(enemy.Player)) { enemy.SwitchState(enemy.ChaseState); return; }
            if (_timer >= SEARCH_TIME) enemy.SwitchState(enemy.PatrolState);
        }
        public override void Exit() { }
    }
}