using UnityEngine;

public class SimpleIngredient : MonoBehaviour, IIngredient
{
    public bool CanBePlated()
    {
        return true;
    }

    public bool IsDesirable()
    {
        return true;
    }
}
