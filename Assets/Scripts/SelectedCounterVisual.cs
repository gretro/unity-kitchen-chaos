using Assets.Scripts;
using UnityEngine;

public partial class ClearCounter : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    [SerializeField]
    private GameObject selectedCounterVisual;

    public void Select()
    {
        this.selectedCounterVisual.SetActive(true);
    }

    public void Unselect()
    {
        this.selectedCounterVisual.SetActive(false);
    }
}
