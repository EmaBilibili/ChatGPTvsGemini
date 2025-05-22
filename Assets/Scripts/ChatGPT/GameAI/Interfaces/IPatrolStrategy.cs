using UnityEngine;
namespace GameAI
{
    public interface IPatrolStrategy { Vector3 GetNextDestination(); void OnDebugDraw(); }
}