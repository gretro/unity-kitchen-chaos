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
    [SerializeField] private EventQueue eventQueue;
    [SerializeField] private GameStateBus gameBus;

    private State gameState;

    private float timeRemaining = 0f;
    private float tick = 0f;

    private int score = 0;

    private void Start()
    {
        gameState = State.WaitingToStart;
        timeRemaining = waitingToStartInSeconds;
        tick = 0f;
    }

    private void OnEnable()
    {
        gameBus.GameStateQuery.QueryHandler = () => new GameState() { State = gameState, TimeRemaining = timeRemaining };
        gameBus.ScoreQuery.QueryHandler = () => score;
        eventQueue.OnEventDispatched += OnEventDispatched;
    }

    private void OnDisable()
    {
        gameBus.GameStateQuery.QueryHandler = null;
        gameBus.ScoreQuery.QueryHandler = null;
        eventQueue.OnEventDispatched -= OnEventDispatched;
    }

    private void OnEventDispatched(object sender, GameEvent e)
    {
        if (e.EventName == EventQueue.OnDeliverySuccess)
        {
            score++;
        }
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

                    gameBus.NotifyGameStateChanged(new GameState() { State = gameState, TimeRemaining = timeRemaining });
                }
                break;

            case State.Countdown:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    Debug.Log("Game State: Countdown -> Playing");
                    gameState = State.Playing;
                    timeRemaining = playTimeInSeconds;

                    gameBus.NotifyGameStateChanged(new GameState() { State = gameState, TimeRemaining = timeRemaining });
                }
                break;

            case State.Playing:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    Debug.Log("Game State: Playing -> GameOver");
                    gameState = State.GameOver;
                    timeRemaining = 0f;

                    gameBus.NotifyGameStateChanged(new GameState() { State = gameState, TimeRemaining = timeRemaining });
                }
                break;

            case State.GameOver:
                break;
        }

        tick += Time.deltaTime;
        if (tick > 1f)
        {
            tick = 0;
        }
    }
}
