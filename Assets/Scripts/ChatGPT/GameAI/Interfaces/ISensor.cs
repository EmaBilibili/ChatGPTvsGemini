using UnityEngine;
namespace GameAI
{
    public interface ISensor { bool Detect(Transform target); void OnDebugDraw(); }
}