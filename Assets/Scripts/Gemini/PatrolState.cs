// PatrolState.cs
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IEnemyState
{
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;

    public void EnterState(EnemyAI controller)
    {
        // Debug.Log("Entrando en Estado de Patrulla");
        controller.Agent.speed = controller.patrolSpeed;
        controller.Agent.isStopped = false; // Asegurarse de que no esté detenido
        waiting = false;
        waitTimer = 0f;
        SetNextDestination(controller);
    }

    public void UpdateState(EnemyAI controller)
    {
        // 1. Prioridad: Comprobar si ve al jugador
        if (controller.CanSeePlayer())
        {
            controller.ChangeState(controller.chaseState);
            return; // Salir del Update actual
        }

        // 2. Lógica de patrullaje
        if (waiting)
        {
            // Está esperando en un punto
            waitTimer += Time.deltaTime;
            if (waitTimer >= controller.patrolWaitTime)
            {
                waiting = false;
                SetNextDestination(controller);
            }
        }
        else
        {
            // Se está moviendo hacia el destino
            // Comprobar si ha llegado (considerando stoppingDistance)
            if (!controller.Agent.pathPending && controller.Agent.remainingDistance <= controller.Agent.stoppingDistance)
            {
                // Ha llegado al destino, empezar a esperar
                waiting = true;
                waitTimer = 0f;
                // Opcional: Detener al agente explícitamente si se desea que no "deslice"
                // controller.Agent.isStopped = true;
            }
        }
    }

    public void ExitState(EnemyAI controller)
    {
        // Limpiar timers o flags si es necesario al salir del estado
        // Debug.Log("Saliendo del Estado de Patrulla");
        // No es necesario detener al agente aquí, el siguiente estado lo configurará
    }

    private void SetNextDestination(EnemyAI controller)
    {
        // controller.Agent.isStopped = false; // Asegurarse de que pueda moverse

        if (controller.patrolType == EnemyAI.PatrolType.Waypoints)
        {
            // Patrulla por Waypoints
            if (controller.waypoints == null || controller.waypoints.Count == 0)
            {
                Debug.LogWarning($"'{controller.gameObject.name}': No hay waypoints definidos para patrullar.");
                // Podría cambiar a patrulla aleatoria o quedarse quieto
                controller.Agent.SetDestination(controller.transform.position); // Se queda quieto
                return;
            }

            // Ir al siguiente waypoint (cíclico)
            currentWaypointIndex = (currentWaypointIndex + 1) % controller.waypoints.Count;
            controller.Agent.SetDestination(controller.waypoints[currentWaypointIndex].position);
        }
        else // Patrulla Aleatoria
        {
            Vector3 randomDestination = controller.GetRandomNavMeshPointInRadius();
            controller.Agent.SetDestination(randomDestination);
        }
    }
}