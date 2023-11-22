using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField]
    private List<MealSO> orderableMeals = new();

    [SerializeField]
    private float orderIntervalInSeconds = 5f;

    [SerializeField]
    private int maxPendingOrders = 4;

    private float orderTimer = 0f;
    private readonly List<MealSO> pendingOrders = new();

    private void Update()
    {
        if (pendingOrders.Count >= maxPendingOrders)
        {
            return;
        }

        orderTimer += Time.deltaTime;
        if (orderTimer >= orderIntervalInSeconds)
        {
            orderTimer = 0f;

            var order = orderableMeals[Random.Range(0, orderableMeals.Count)];
            pendingOrders.Add(order);

            Debug.Log($"New order: {order.mealName}");
        }
    }

    public void DeliverOrder(IEnumerable<ServingPlateIngredient> ingredients)
    {
        foreach (var pendingOrder in pendingOrders)
        {
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
                Debug.Log($"Order for {pendingOrder.mealName} delivered!");

                return;
            }
        }

        Debug.Log("No order delivery match found");
    }
}
