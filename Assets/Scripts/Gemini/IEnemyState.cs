// IEnemyState.cs
using UnityEngine;

// Define el contrato para todos los estados de la IA
public interface IEnemyState
{
    // Se llama al entrar en este estado
    void EnterState(EnemyAI controller);

    // Se llama en cada frame mientras se está en este estado
    void UpdateState(EnemyAI controller);

    // Se llama al salir de este estado
    void ExitState(EnemyAI controller);
}