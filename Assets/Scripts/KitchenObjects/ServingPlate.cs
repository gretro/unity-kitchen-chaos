using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServingPlate : MonoBehaviour, IKitchenObjectHolder
{
    [SerializeField] private KitchenObjectSO breadDefinition;
    [SerializeField] private GameObject bread;

    [SerializeField] private KitchenObjectSO cabbageDefinition;
    [SerializeField] private GameObject cabbage;

    [SerializeField] private KitchenObjectSO pattyDefinition;
    [SerializeField] private GameObject cookedPatty;
    [SerializeField] private GameObject burnedPatty;

    [SerializeField] private KitchenObjectSO cheeseDefinition;
    [SerializeField] private GameObject cheese;

    [SerializeField] private KitchenObjectSO tomatoDefinition;
    [SerializeField] private GameObject tomato;

    [SerializeField] private GameObject ingredientsUI;
    [SerializeField] private GameObject ingredientTemplate;

    private Dictionary<string, GameObject> ingredientVisuals;

    private readonly Dictionary<string, ServingPlateIngredient> heldIngredients = new();

    private const string NOT_DESIRABLE = "undesirable";
    private const string DESIRABLE = "desirable";

    private void Start()
    {
        ingredientVisuals = new Dictionary<string, GameObject>()
        {
            { $"{breadDefinition.objectName}_{DESIRABLE}", bread },
            { $"{cabbageDefinition.objectName}_{DESIRABLE}", cabbage },
            { $"{pattyDefinition.objectName}_{DESIRABLE}", cookedPatty },
            { $"{pattyDefinition.objectName}_{NOT_DESIRABLE}", burnedPatty },
            { $"{cheeseDefinition.objectName}_{DESIRABLE}", cheese },
            { $"{tomatoDefinition.objectName}_{DESIRABLE}", tomato },
        };

        foreach (var ingredientVisual in ingredientVisuals)
        {
            ingredientVisual.Value.SetActive(false);
        }
    }

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        if (!kitchenObject.ObjectType.isIngredient)
        {
            return false;
        }

        if (heldIngredients.ContainsKey(kitchenObject.ObjectType.objectName))
        {
            return false;
        }

        if (kitchenObject.TryGetComponent<IIngredient>(out var ingredient))
        {
            return ingredient.CanBePlated();
        }

        return false;

    }

    public bool HoldsObject()
    {
        return heldIngredients.Count > 0;
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            return;
        }

        if (kitchenObject.TryGetComponent<IIngredient>(out var ingredient))
        {
            var servingPlateIngredient = new ServingPlateIngredient()
            {
                IsDesirable = ingredient.IsDesirable(),
                KitchenObjectSO = kitchenObject.ObjectType,
            };

            heldIngredients.Add(kitchenObject.ObjectType.objectName, servingPlateIngredient);

            var key = $"{kitchenObject.ObjectType.objectName}_{(servingPlateIngredient.IsDesirable ? DESIRABLE : NOT_DESIRABLE)}";
            var gameObject = ingredientVisuals[key];

            if (gameObject != null)
            {
                gameObject.SetActive(true);
                AddIngredientToUI(servingPlateIngredient);
            }

            GameObject.Destroy(kitchenObject.gameObject);
        }
    }

    private void AddIngredientToUI(ServingPlateIngredient ingredient)
    {
        var instance = GameObject.Instantiate(ingredientTemplate, ingredientsUI.transform);
        instance.name = $"{ingredient.KitchenObjectSO.name} Icon";
        var images = instance.GetComponentsInChildren<UnityEngine.UI.Image>();

        var icon = images.Where(img => img.CompareTag("Icon")).FirstOrDefault();
        if (icon != null)
        {
            icon.sprite = ingredient.KitchenObjectSO.sprite;
        }

        instance.SetActive(true);
    }
}
