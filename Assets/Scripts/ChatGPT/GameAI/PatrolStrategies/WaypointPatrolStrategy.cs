using UnityEngine;
namespace GameAI
{
    public class WaypointPatrolStrategy : IPatrolStrategy
    {
        private readonly Transform[] _waypoints;
        private int _index;
        public WaypointPatrolStrategy(Transform[] waypoints) { _waypoints = waypoints; _index = 0; }
        public Vector3 GetNextDestination()
        {
            if (_waypoints == null || _waypoints.Length == 0) return Vector3.zero;
            Vector3 next = _waypoints[_index].position;
            _index = (_index + 1) % _waypoints.Length;
            return next;
        }
#if UNITY_EDITOR
        public void OnDebugDraw()
        {
            if (_waypoints == null) return;
            for (int i = 0; i < _waypoints.Length; i++)
            {
                int next = (i + 1) % _waypoints.Length;
                Debug.DrawLine(_waypoints[i].position, _waypoints[next].position, Color.green);
            }
        }
#else
        public void OnDebugDraw() { }
#endif
    }
}