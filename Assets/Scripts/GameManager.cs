using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        WaitingToStart,
        Countdown,
        Playing,
        GameOver
    }

    [Header("Configuration")]
    [SerializeField] private float waitingToStartInSeconds = 1f;
    [SerializeField] private float countdownInSeconds = 3f;
    [SerializeField] private float playTimeInSeconds = 90f;

    [Header("Events/Bus")]
    [SerializeField] private GameStateBus gameBus;

    private State gameState;

    private float timeRemaining = 0f;
    private float tick = 0f;

    private void Start()
    {
        gameState = State.WaitingToStart;
        timeRemaining = waitingToStartInSeconds;
        tick = 0f;
    }

    private void OnEnable()
    {
        gameBus.GameStateQuery.QueryHandler = () => gameState;
        gameBus.Tick(gameState, timeRemaining);
    }

    private void OnDisable()
    {
        gameBus.GameStateQuery.QueryHandler = null;
    }

    private void Update()
    {
        switch (gameState)
        {
            case State.WaitingToStart:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    Debug.Log("Game State: WaitingToStart -> Countdown");
                    gameState = State.Countdown;
                    timeRemaining = countdownInSeconds;
                }
                break;

            case State.Countdown:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    Debug.Log("Game State: Countdown -> Playing");
                    gameState = State.Playing;
                    timeRemaining = playTimeInSeconds;
                }
                break;

            case State.Playing:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    Debug.Log("Game State: Playing -> GameOver");
                    gameState = State.GameOver;
                    timeRemaining = 0f;
                }
                break;

            case State.GameOver:
                break;
        }

        tick += Time.deltaTime;
        if (tick > 1f)
        {
            tick = 0;
            gameBus.Tick(gameState, timeRemaining);
        }
    }
}
