using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateBus", menuName = "KitchenChaos/GameStateBus")]
public class GameStateBus : ScriptableObject
{
    public event EventHandler<GameTickEvent> GameTick;

    public Query<GameManager.State> GameStateQuery { get; private set; } = new("GameStateQuery");

    public void Tick(GameManager.State currentState, float timeRemaining)
    {
        GameTick?.Invoke(this, new GameTickEvent { CurrentState = currentState, TimeRemaining = timeRemaining });
    }
}

public class GameTickEvent : EventArgs
{
    public GameManager.State CurrentState { get; set; }

    public float TimeRemaining { get; set; }
}