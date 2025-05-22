// EnemyAI.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Para listas

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    // --- Parámetros Configurables en Inspector ---
    [Header("Referencias")]
    [SerializeField] private Transform playerTransform; // Arrastra al jugador aquí
    [SerializeField] private LayerMask detectionObstacles; // Capas que bloquean la visión (ej. Paredes)

    [Header("Patrullaje")]
    public PatrolType patrolType = PatrolType.Waypoints;
    public List<Transform> waypoints = new List<Transform>(); // Para patrulla por waypoints
    public float patrolRadius = 10f; // Para patrulla aleatoria
    public float patrolWaitTime = 3f; // Tiempo de espera en cada punto/destino
    public float patrolSpeed = 2.5f;

    [Header("Detección")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;

    [Header("Persecución")]
    public float chaseSpeed = 5f;

    [Header("Búsqueda")]
    public float searchDuration = 5f; // Tiempo buscando antes de volver a patrullar
    public float searchSpeed = 3f; // Puedes usar patrolSpeed o una velocidad específica

    // --- Referencias Internas ---
    public NavMeshAgent Agent { get; private set; }
    public Transform PlayerTransform => playerTransform; // Propiedad pública para acceder al jugador
    public Vector3 LastKnownPlayerPosition { get; set; } // Última pos. conocida

    // --- Máquina de Estados ---
    private IEnemyState currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public SearchState searchState;

    // --- Variables Privadas ---
    private Vector3 startingPosition; // Para patrulla aleatoria

    // --- Enums ---
    public enum PatrolType { Waypoints, Random }

    // --- Inicialización ---
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (!playerTransform)
        {
            Debug.LogError($"'{gameObject.name}': No se ha asignado el Transform del jugador.");
            // Opcional: Buscar jugador por tag
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if(playerObject) playerTransform = playerObject.transform;
            else enabled = false; // Desactivar si no hay jugador
        }

        // Guardar posición inicial para patrulla aleatoria
        startingPosition = transform.position;

        // Crear instancias de los estados
        patrolState = new PatrolState();
        chaseState = new ChaseState();
        searchState = new SearchState();
    }

    private void Start()
    {
        // Establecer el estado inicial
        ChangeState(patrolState);
    }

    // --- Bucle Principal ---
    private void Update()
    {
        // Delega la lógica al estado actual
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    // --- Cambio de Estado ---
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
        // Debug.Log($"Cambiando a estado: {newState.GetType().Name}"); // Útil para depurar
    }

    // --- Lógica de Detección ---
    public bool CanSeePlayer()
    {
        if (!playerTransform) return false; // No hay jugador

        Vector3 directionToPlayer = (playerTransform.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        // 1. Comprobar Radio de Visión
        if (distanceToPlayer > viewRadius)
        {
            return false; // Demasiado lejos
        }

        // Normalizar dirección para cálculos de ángulo y raycast
        directionToPlayer.Normalize();

        // 2. Comprobar Ángulo de Visión
        // Vector3.Angle necesita el vector 'forward' del enemigo
        if (Vector3.Angle(transform.forward, directionToPlayer) > viewAngle / 2)
        {
            return false; // Fuera del ángulo de visión
        }

        // 3. Comprobar Línea de Visión (Obstáculos)
        // Usamos la posición de los ojos (aproximada) para el Raycast
        Vector3 eyePosition = transform.position + Vector3.up * Agent.height * 0.75f; // Ajusta la altura si es necesario
        Vector3 playerEyePosition = playerTransform.position + Vector3.up; // Aprox. cabeza del jugador

        // Necesitamos la dirección desde los ojos del enemigo a los del jugador
        Vector3 directionToPlayerEyes = (playerEyePosition - eyePosition);

        // Distancia máxima del raycast es la distancia al jugador
        float rayDistance = Vector3.Distance(eyePosition, playerEyePosition);

        // Physics.Raycast es más eficiente que Linecast para esto
        if (Physics.Raycast(eyePosition, directionToPlayerEyes.normalized, rayDistance, detectionObstacles))
        {
           //Debug.DrawRay(eyePosition, directionToPlayerEyes.normalized * rayDistance, Color.yellow); // Para debug
           return false; // Obstáculo detectado
        }

        // Si pasa todas las comprobaciones, el jugador es visible
        //Debug.DrawRay(eyePosition, directionToPlayerEyes.normalized * rayDistance, Color.green); // Para debug
        return true;
    }

    // --- Lógica de Patrulla Aleatoria ---
    public Vector3 GetRandomNavMeshPointInRadius()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startingPosition; // Centrar en la posición inicial
        NavMeshHit navHit;

        // Busca el punto más cercano en el NavMesh dentro de un radio de búsqueda (ej. 5 unidades)
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius * 0.5f, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            // Si no encuentra un punto (poco probable si el radio es razonable y hay NavMesh),
            // devuelve la posición inicial o la actual para evitar errores.
            return startingPosition;
        }
    }

    // --- Gizmos para Visualización en Editor ---
    private void OnDrawGizmosSelected()
    {
        // Dibujar radio de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Dibujar ángulo de visión
        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, false);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        // Opcional: Dibujar radio de patrulla aleatoria
        if(patrolType == PatrolType.Random)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startingPosition, patrolRadius); // Usar startingPosition si está disponible
        }

        // Opcional: Dibujar línea hacia el jugador si está a la vista (solo en Play Mode)
        if (Application.isPlaying && playerTransform != null)
        {
            if (CanSeePlayer())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + Vector3.up * Agent.height * 0.75f, playerTransform.position + Vector3.up);
            } else {
                 Vector3 directionToPlayer = (playerTransform.position - (transform.position + Vector3.up * Agent.height * 0.75f));
                 Gizmos.color = Color.red;
                 Gizmos.DrawRay(transform.position + Vector3.up * Agent.height * 0.75f, directionToPlayer.normalized * viewRadius);
            }
        }
    }

    // Helper para Gizmos del ángulo
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}