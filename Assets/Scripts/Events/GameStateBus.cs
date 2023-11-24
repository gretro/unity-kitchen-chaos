using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateBus", menuName = "KitchenChaos/GameStateBus")]
public class GameStateBus : ScriptableObject
{
    public event EventHandler<GameState> OnGameStateUpdated;

    public Query<GameState> GameStateQuery { get; private set; } = new("GameStateQuery");

    public Query<int> ScoreQuery { get; private set; } = new("ScoreQuery");

    public void NotifyGameStateChanged(GameState gameState)
    {
        OnGameStateUpdated?.Invoke(this, gameState);
    }
}

public struct GameState
{
    public GameManager.State State { get; set; }
    public float TimeRemaining { get; set; }
}