// ChaseState.cs
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyAI controller)
    {
        // Debug.Log("Entrando en Estado de Persecución");
        controller.Agent.speed = controller.chaseSpeed;
        controller.Agent.isStopped = false; // Asegurarse de que pueda moverse
        // Podrías añadir efectos visuales/sonidos de alerta aquí
    }

    public void UpdateState(EnemyAI controller)
    {
        // 1. Comprobar si aún ve o está cerca del jugador
        if (controller.CanSeePlayer())
        {
            // Actualizar destino a la posición actual del jugador
            controller.Agent.SetDestination(controller.PlayerTransform.position);
            // Guardar la última posición conocida por si lo pierde
            controller.LastKnownPlayerPosition = controller.PlayerTransform.position;
        }
        else
        {
            // Perdió al jugador, cambiar a estado de búsqueda
            // La última posición ya fue guardada en el frame anterior donde sí lo veía
             // Comprobar si la última posición conocida es válida (no es Vector3.zero por defecto)
            if (controller.LastKnownPlayerPosition != Vector3.zero)
            {
                controller.ChangeState(controller.searchState);
            }
            else
            {
                // Si por alguna razón no hay última posición válida, volver a patrullar
                 Debug.LogWarning($"'{controller.gameObject.name}': No hay última posición conocida, volviendo a patrullar.");
                controller.ChangeState(controller.patrolState);
            }
        }

        // Aquí podrías añadir lógica de ataque si está lo suficientemente cerca
        // if (Vector3.Distance(controller.transform.position, controller.PlayerTransform.position) < controller.attackRange)
        // {
        //    controller.ChangeState(controller.attackState); // Si tuvieras un estado de ataque
        // }
    }

    public void ExitState(EnemyAI controller)
    {
        // Debug.Log("Saliendo del Estado de Persecución");
        // No detener al agente aquí, el estado Search o Patrol lo manejarán
        // Podrías detener animaciones de persecución, etc.
    }
}