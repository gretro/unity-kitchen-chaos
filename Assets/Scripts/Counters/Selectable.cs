using UnityEngine;

public class Selectable : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField]
    private GameObject selectedVisual;

    public void Select()
    {
        selectedVisual.SetActive(true);
    }

    public void Unselect()
    {
        selectedVisual.SetActive(false);
    }
}