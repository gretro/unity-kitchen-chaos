using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler<MealChangedEvent> OnMealAdded;
    public event EventHandler<MealChangedEvent> OnMealRemoved;

    public class MealChangedEvent : EventArgs
    {
        public MealSO Meal { get; set; }
        public int Index { get; set; }
    }

    [SerializeField]
    private List<MealSO> orderableMeals = new();

    [SerializeField]
    private float orderIntervalInSeconds = 5f;

    [SerializeField]
    private int maxPendingOrders = 4;

    [Header("Events/Bus")]
    [SerializeField] private EventQueue eventQueue;
    [SerializeField] private GameStateBus gameStateBus;

    private float orderTimer = 0f;
    private readonly List<MealSO> pendingOrders = new();

    private GameManager.State gameState;

    private void Start()
    {
        gameStateBus.OnGameStateUpdated += OnGameStateUpdated;
    }

    private void OnGameStateUpdated(object sender, GameState newState)
    {
        this.gameState = newState.State;
    }

    private void Update()
    {
        if (this.gameState != GameManager.State.Playing)
        {
            return;
        }

        if (pendingOrders.Count >= maxPendingOrders)
        {
            return;
        }

        orderTimer += Time.deltaTime;
        if (orderTimer >= orderIntervalInSeconds)
        {
            orderTimer = 0f;
            GenerateOrder();
        }
    }

    private void GenerateOrder()
    {
        var order = orderableMeals[UnityEngine.Random.Range(0, orderableMeals.Count)];
        pendingOrders.Add(order);

        this.OnMealAdded?.Invoke(this, new MealChangedEvent { Meal = order, Index = pendingOrders.Count - 1 });
    }

    public IEnumerable<MealSO> GetPendingOrders()
    {
        return pendingOrders;
    }

    public void DeliverOrder(GameObject origin, IEnumerable<ServingPlateIngredient> ingredients)
    {
        for (var i = 0; i < pendingOrders.Count; i++)
        {
            var pendingOrder = pendingOrders[i];

            if (ingredients.Count() != pendingOrder.ingredients.Count)
            {
                continue;
            }

            var isMatch = true;
            foreach (var ingredient in ingredients)
            {
                // Handling burnt ingredients
                if (!ingredient.IsDesirable)
                {
                    isMatch = false;
                    break;
                }

                if (!pendingOrder.ingredients.Contains(ingredient.KitchenObjectSO))
                {
                    isMatch = false;
                    break;
                }
            }


            if (isMatch)
            {
                pendingOrders.Remove(pendingOrder);
                this.OnMealRemoved?.Invoke(this, new MealChangedEvent { Meal = pendingOrder, Index = i });

                Debug.Log($"Order for {pendingOrder.mealName} delivered!");

                eventQueue.DispatchEvent(EventQueue.OnDeliverySuccess, origin);
                return;
            }
        }

        eventQueue.DispatchEvent(EventQueue.OnDeliveryFailure, origin);
        Debug.Log("No order delivery match found");
    }
}
