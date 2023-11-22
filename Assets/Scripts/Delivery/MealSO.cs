using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Meal", menuName = "KitchenChaos/Meal")]
public class MealSO : ScriptableObject
{
    public List<KitchenObjectSO> ingredients = new();
    public string mealName;
}
