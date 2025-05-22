using UnityEngine;
namespace GameAI
{
    public interface IState { void Enter(); void Execute(); void Exit(); }
}