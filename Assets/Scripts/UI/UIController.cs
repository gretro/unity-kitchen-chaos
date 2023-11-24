using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
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

    private GameManager.State? lastState = null;

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
        lblGameState = uiDocument.rootVisualElement.Q<Label>("GameState");
        gameStateBus.GameTick += HandleGameTick;

        UpdateGameState(gameStateBus.GameStateQuery.RaiseQuery(), 0f);
        lblTimer.text = "";
    }

    private void OnDisable()
    {
        deliveryManager.OnMealAdded -= DeliveryManager_OnMealAdded;
        deliveryManager.OnMealRemoved -= DeliveryManager_OnMealRemoved;

        gameStateBus.GameTick -= HandleGameTick;
    }


    private void DeliveryManager_OnMealAdded(object sender, DeliveryManager.MealChangedEvent e)
    {
        AddMealOrder(e.Meal);
    }

    private void DeliveryManager_OnMealRemoved(object sender, DeliveryManager.MealChangedEvent e)
    {
        RemoveMealOrder(e.Index);
    }

    private void HandleGameTick(object sender, GameTickEvent e)
    {
        if (lastState != e.CurrentState)
        {
            lastState = e.CurrentState;
        }

        UpdateGameState(e.CurrentState, e.TimeRemaining);

        if (e.CurrentState == GameManager.State.Playing || e.CurrentState == GameManager.State.GameOver)
        {
            UpdateTimer(e.TimeRemaining);
        }
        else
        {
            lblTimer.text = "";
        }
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

    private void UpdateGameState(GameManager.State? state, float remainingTime)
    {
        var text = "";

        switch (state)
        {
            case GameManager.State.WaitingToStart:
                text = "Waiting for all players";
                break;
            case GameManager.State.Countdown:
                text = $"{Mathf.FloorToInt(remainingTime) + 1}";
                break;
            case GameManager.State.GameOver:
                text = "Game Over";
                break;
        }

        lblGameState.text = text;
    }

    private void UpdateTimer(float remainingTime)
    {
        var minutes = Mathf.FloorToInt(remainingTime / 60);
        var seconds = Mathf.FloorToInt(remainingTime % 60);

        var time = $"{minutes}:{seconds:00}";

        lblTimer.text = time;
    }
}
