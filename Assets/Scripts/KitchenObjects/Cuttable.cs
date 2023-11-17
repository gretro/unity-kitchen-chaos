using UnityEngine;

public class Cuttable : MonoBehaviour
{
    [Header("Cutting")]
    [SerializeField]
    private CuttingRecipeSO recipe;

    [Header("Visuals")]
    [SerializeField]
    private GameObject pristineVisual;
    [SerializeField]
    private GameObject cutVisual;

    public bool IsCut { get; private set; }

    public void Cut()
    {
        pristineVisual.SetActive(false);
        cutVisual.SetActive(true);

        IsCut = true;
    }

    public CuttingRecipeSO GetRecipe()
    {
        return recipe;
    }
}
