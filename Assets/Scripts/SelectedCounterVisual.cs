using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedCounterVisual;

    private ClearCounter counter;

    public void Start()
    {
        counter = GetComponent<ClearCounter>();
        Player.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
    }

    private void OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        var isSelected = e.SelectedCounter == counter;

        selectedCounterVisual.SetActive(isSelected);
    }
}
