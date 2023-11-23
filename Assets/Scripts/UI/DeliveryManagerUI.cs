using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DeliveryManagerUI : MonoBehaviour
{
    [Header("Templates")]
    [SerializeField]
    private VisualTreeAsset mealTemplate;

    [SerializeField]
    private VisualTreeAsset ingredientTemplate;

    [Header("Manager")]
    [SerializeField]
    private DeliveryManager deliveryManager;

    private UIDocument uiDocument;
    private VisualElement mealsContainer;

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
    }

    private void OnDisable()
    {
        deliveryManager.OnMealAdded -= DeliveryManager_OnMealAdded;
        deliveryManager.OnMealRemoved -= DeliveryManager_OnMealRemoved;
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
}
