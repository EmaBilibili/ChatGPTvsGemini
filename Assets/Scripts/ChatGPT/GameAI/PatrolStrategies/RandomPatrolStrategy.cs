using UnityEngine;
namespace GameAI
{
    public class RandomPatrolStrategy : IPatrolStrategy
    {
        private readonly Transform _origin; private readonly float _radius;
        public RandomPatrolStrategy(Transform origin, float radius) { _origin = origin; _radius = radius; }
        public Vector3 GetNextDestination()
        {
            Vector2 random = Random.insideUnitCircle * _radius;
            return _origin.position + new Vector3(random.x, 0, random.y);
        }
#if UNITY_EDITOR
        public void OnDebugDraw()
        { UnityEditor.Handles.color = Color.blue; UnityEditor.Handles.DrawWireDisc(_origin.position, Vector3.up, _radius); }
#else
        public void OnDebugDraw() { }
#endif
    }
}