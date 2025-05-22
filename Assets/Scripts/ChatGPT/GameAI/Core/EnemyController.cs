using UnityEngine;
using UnityEngine.AI;

namespace GameAI
{
    /// <summary>
    /// Componente central que orquesta sensor, estrategia de patrulla y estados.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        //────────────────── Inspector
        [Header("General")]
        [SerializeField] private Transform player;

        [Header("Velocidades")]
        public float patrolSpeed = 3.5f;
        public float chaseSpeed = 5.5f;

        [Header("Patrulla Waypoints (opcional)")]
        public Transform[] waypoints;

        [Header("Patrulla Aleatoria")]
        public float randomPatrolRadius = 10f;
        public bool useRandomPatrol = false;

        [Header("Sensor de Visión")]
        public VisionSensor visionSettings = new VisionSensor();

        //────────────────── Propiedades públicas
        public Transform Player => player;
        public NavMeshAgent Agent { get; private set; }
        public IPatrolStrategy PatrolStrategy { get; private set; }
        public VisionSensor Sensor { get; private set; }
        public Vector3 LastKnownPosition { get; set; }

        //────────────────── Estados
        public PatrolState PatrolState { get; private set; }
        public ChaseState ChaseState { get; private set; }
        public SearchState SearchState { get; private set; }
        private IState _currentState;

        //────────────────── Mono
        private void Awake()
        {
            // Cache del NavMeshAgent
            Agent = GetComponent<NavMeshAgent>();

            // Configuración del sensor
            Sensor = visionSettings;
            Sensor.Init(transform);

            // Selección de estrategia de patrulla
            PatrolStrategy = (useRandomPatrol || waypoints == null || waypoints.Length == 0)
                ? new RandomPatrolStrategy(transform, randomPatrolRadius)
                : new WaypointPatrolStrategy(waypoints) as IPatrolStrategy;

            // Instancia de estados
            PatrolState  = new PatrolState(this);
            ChaseState   = new ChaseState(this);
            SearchState  = new SearchState(this);

            // Parámetros por defecto del agent
            Agent.speed = patrolSpeed;
            Agent.stoppingDistance = 0.1f;
            Agent.updateRotation = true;
            Agent.autoBraking = true;
        }

        private void Start() => SwitchState(PatrolState);
        private void Update() => _currentState?.Execute();

        public void SwitchState(IState next)
        {
            if (next == null || next == _currentState) return;
            _currentState?.Exit();
            _currentState = next;
            _currentState.Enter();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Sensor?.OnDebugDraw();
            PatrolStrategy?.OnDebugDraw();
        }
#endif
    }
}
