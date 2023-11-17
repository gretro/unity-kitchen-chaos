using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image progressBar;


    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateProgress(float progress, float total)
    {
        var barProgress = 0f;

        if (total > 0)
        {
            barProgress = progress / total;
        }

        progressBar.fillAmount = Mathf.Clamp(barProgress, 0, 1);
    }

    public void Reset()
    {
        progressBar.fillAmount = 0;
    }
}
