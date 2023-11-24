using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private const float GAME_STATE_ANIMATION_TIME = 1f;
    private const string GAME_STATE_CLASS = "game-state-tock";
    private const string GAMEOVER_TITLE_ANIMATION = "game-over-title-tock";

    [Header("Templates")]
    [SerializeField]
    private VisualTreeAsset mealTemplate;

    [SerializeField]
    private VisualTreeAsset ingredientTemplate;

    [Header("Manager")]
    [SerializeField]
    private DeliveryManager deliveryManager;

    [Header("Events/Bus")]
    [SerializeField] private GameStateBus gameStateBus;

    private UIDocument uiDocument;
    private VisualElement mealsContainer;
    private Label lblTimer;
    private Label lblGameState;

    private VisualElement gameOverContainer;
    private Label lblGameOverTitle;
    private Label lblScore;

    private float checkStateTick = 0f;
    private float animationTick = GAME_STATE_ANIMATION_TIME * 0.75f;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        mealsContainer = uiDocument.rootVisualElement.Q<VisualElement>("OrdersHolder");

        mealsContainer.Clear();
        var pendingOrders = deliveryManager.GetPendingOrders();
        foreach (var order in pendingOrders)
        {
            AddMealOrder(order);
        }

        deliveryManager.OnMealAdded += DeliveryManager_OnMealAdded;
        deliveryManager.OnMealRemoved += DeliveryManager_OnMealRemoved;

        lblTimer = uiDocument.rootVisualElement.Q<Label>("Timer");
        lblTimer.text = "";

        lblGameState = uiDocument.rootVisualElement.Q<Label>("GameState");
        var gameState = gameStateBus.GameStateQuery.RaiseQuery();
        UpdateGameState(gameState);

        gameOverContainer = uiDocument.rootVisualElement.Q<VisualElement>("GameOverOverlay");
        gameOverContainer.style.display = DisplayStyle.None;

        lblGameOverTitle = uiDocument.rootVisualElement.Q<Label>("GameOverTitle");
        lblScore = uiDocument.rootVisualElement.Q<Label>("Score");
    }

    private void OnDisable()
    {
        deliveryManager.OnMealAdded -= DeliveryManager_OnMealAdded;
        deliveryManager.OnMealRemoved -= DeliveryManager_OnMealRemoved;
    }

    private void Update()
    {
        checkStateTick += Time.deltaTime;
        if (checkStateTick > 0.5f)
        {
            var gameState = gameStateBus.GameStateQuery.RaiseQuery();
            UpdateGameState(gameState);
            UpdateTimer(gameState);

            checkStateTick = 0f;
        }

        animationTick += Time.deltaTime;
        if (animationTick >= GAME_STATE_ANIMATION_TIME)
        {
            animationTick = 0f;
            lblGameState.ToggleInClassList(GAME_STATE_CLASS);
            lblGameOverTitle.ToggleInClassList(GAMEOVER_TITLE_ANIMATION);
        }
    }

    private void DeliveryManager_OnMealAdded(object sender, DeliveryManager.MealChangedEvent e)
    {
        AddMealOrder(e.Meal);
    }

    private void DeliveryManager_OnMealRemoved(object sender, DeliveryManager.MealChangedEvent e)
    {
        RemoveMealOrder(e.Index);
    }

    private void AddMealOrder(MealSO mealOrder)
    {
        var mealVisual = mealTemplate.Instantiate();

        var controller = new MealController();
        mealVisual.userData = controller;

        controller.Init(ingredientTemplate, mealVisual);
        controller.BindData(mealOrder);

        mealsContainer.Add(mealVisual);
    }

    private void RemoveMealOrder(int index)
    {
        mealsContainer.Children().ElementAt(index).RemoveFromHierarchy();
    }

    private void UpdateGameState(GameState gameState)
    {
        var text = "";

        switch (gameState.State)
        {
            case GameManager.State.WaitingToStart:
                text = "Waiting";
                break;
            case GameManager.State.Countdown:
                text = $"{Mathf.FloorToInt(gameState.TimeRemaining - 0.01f) + 1}";
                break;
        }

        lblGameState.text = text;

        if (gameState.State == GameManager.State.GameOver)
        {
            gameOverContainer.style.display = DisplayStyle.Flex;
            lblScore.text = $"{gameStateBus.ScoreQuery.RaiseQuery()}";
        }
    }

    private void UpdateTimer(GameState gameState)
    {
        if (gameState.State != GameManager.State.Playing && gameState.State != GameManager.State.GameOver)
        {
            lblTimer.text = "";
            return;
        }

        var minutes = Mathf.FloorToInt(gameState.TimeRemaining / 60);
        var seconds = Mathf.FloorToInt(gameState.TimeRemaining % 60);

        var time = $"{minutes}:{seconds:00}";

        lblTimer.text = time;
    }
}
