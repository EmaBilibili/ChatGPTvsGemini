// SearchState.cs
using UnityEngine;
using UnityEngine.AI;

public class SearchState : IEnemyState
{
    private float searchTimer;

    public void EnterState(EnemyAI controller)
    {
       // Debug.Log($"Entrando en Estado de Búsqueda en {controller.LastKnownPlayerPosition}");
        controller.Agent.speed = controller.searchSpeed; // Usar velocidad de búsqueda
        controller.Agent.isStopped = false;
        searchTimer = 0f;

        // Ir a la última posición conocida del jugador
        if (controller.LastKnownPlayerPosition != Vector3.zero) // Asegurarse de que haya una posición válida
        {
            controller.Agent.SetDestination(controller.LastKnownPlayerPosition);
        }
        else
        {
            // Si no hay posición válida (raro, pero posible), volver a patrullar inmediatamente
            Debug.LogWarning($"'{controller.gameObject.name}': Intentando buscar sin posición válida, volviendo a patrullar.");
            controller.ChangeState(controller.patrolState);
        }
    }

    public void UpdateState(EnemyAI controller)
    {
        // 1. Prioridad: Comprobar si vuelve a ver al jugador
        if (controller.CanSeePlayer())
        {
            controller.ChangeState(controller.chaseState);
            return;
        }

        // 2. Actualizar temporizador de búsqueda
        searchTimer += Time.deltaTime;

        // 3. Comprobar si ha llegado al punto de búsqueda O si se acabó el tiempo
        bool reachedDestination = !controller.Agent.pathPending && controller.Agent.remainingDistance <= controller.Agent.stoppingDistance;
        bool timeExpired = searchTimer >= controller.searchDuration;

        if (reachedDestination || timeExpired)
        {
            // Dejar de buscar y volver a patrullar
            // Debug.Log("Búsqueda finalizada (llegó o tiempo agotado), volviendo a patrullar.");
            controller.ChangeState(controller.patrolState);
        }

        // Opcional: Podrías añadir lógica más compleja aquí, como mirar alrededor
        // o moverse a puntos cercanos aleatorios mientras busca.
    }

    public void ExitState(EnemyAI controller)
    {
        // Debug.Log("Saliendo del Estado de Búsqueda");
        // Resetear la última posición conocida para evitar re-entrar a buscar en el mismo sitio si no vio al jugador
        controller.LastKnownPlayerPosition = Vector3.zero;
    }
}