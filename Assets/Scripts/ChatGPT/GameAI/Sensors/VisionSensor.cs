using UnityEngine;
namespace GameAI
{
    [System.Serializable]
    public class VisionSensor : ISensor
    {
        [Header("Rango y Ángulo")]
        public float radius = 15f;
        [Range(0, 360)] public float fov = 110f;
        [Header("Altura del " + nameof(eyeHeight) + " (m)")]
        public float eyeHeight = 1.6f; // ← NUEVO: altura relativa al pivot del enemigo.
        [Header("Raycast bloqueará solo estas capas")]
        public LayerMask obstacleMask;

        private Transform _origin;
        public void Init(Transform origin) => _origin = origin;

        public bool Detect(Transform target)
        {
            if (!_origin || !target) return false;

            // Posición del "ojo" del enemigo.
            Vector3 eyePos = _origin.position + Vector3.up * eyeHeight;

            // Punto al que apuntar: centro del collider si existe.
            Vector3 targetPos = target.position;
            if (target.TryGetComponent<Collider>(out var col))
                targetPos = col.bounds.center;

            Vector3 dirToTarget = targetPos - eyePos;
            if (dirToTarget.sqrMagnitude > radius * radius) return false;

            float angle = Vector3.Angle(_origin.forward, dirToTarget);
            if (angle > fov * 0.5f) return false;

            int mask = obstacleMask.value == 0 ? ~0 : ~obstacleMask; // si no hay mask, colisiona con todo

            if (Physics.Raycast(eyePos, dirToTarget.normalized, out RaycastHit hit, radius, mask))
            {
                return hit.transform == target;
            }
            return false;
        }

#if UNITY_EDITOR
        public void OnDebugDraw()
        {
            if (!_origin) return;
            Vector3 eyePos = _origin.position + Vector3.up * eyeHeight;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(_origin.position, Vector3.up, radius);
            Vector3 left = Quaternion.Euler(0, -fov * 0.5f, 0) * _origin.forward;
            Vector3 right = Quaternion.Euler(0, fov * 0.5f, 0) * _origin.forward;
            Debug.DrawRay(_origin.position, left * radius, Color.yellow);
            Debug.DrawRay(_origin.position, right * radius, Color.yellow);
            Debug.DrawLine(_origin.position, eyePos, Color.cyan); // visualiza la altura del ojo
        }
#else
        public void OnDebugDraw() { }
#endif
    }
}