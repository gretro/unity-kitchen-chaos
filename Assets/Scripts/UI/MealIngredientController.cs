using UnityEngine.UIElements;

public class MealIngredientController
{
    private VisualElement ingredient;

    public void Init(VisualElement container)
    {
        ingredient = container.Q<VisualElement>("Ingredient");
    }

    public void BindData(KitchenObjectSO kitchenObj)
    {
        this.ingredient.style.backgroundImage = new StyleBackground(kitchenObj.sprite);
    }
}
