using UnityEngine.UIElements;

public class MealController
{
    private VisualTreeAsset ingredientTemplate;

    private Label nameLabel;
    private VisualElement ingredientsContainer;

    public void Init(VisualTreeAsset ingredientTemplate, VisualElement container)
    {
        this.ingredientTemplate = ingredientTemplate;

        nameLabel = container.Q<Label>("OrderName");
        ingredientsContainer = container.Q<VisualElement>("Ingredients");
    }

    public void BindData(MealSO meal)
    {
        nameLabel.text = meal.mealName;

        foreach (var ingredient in meal.ingredients)
        {
            var ingredientVisual = ingredientTemplate.Instantiate();

            var ingredientController = new MealIngredientController();
            ingredientVisual.userData = ingredientController;

            ingredientController.Init(ingredientVisual);
            ingredientController.BindData(ingredient);

            ingredientsContainer.Add(ingredientVisual);
        }
    }
}
