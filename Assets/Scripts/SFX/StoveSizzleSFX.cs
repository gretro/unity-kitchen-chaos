using UnityEngine;

public class StoveSizzleSFX : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        eventQueue.OnEventDispatched += OnEventDispatched;
    }

    private void OnDisable()
    {
        eventQueue.OnEventDispatched -= OnEventDispatched;
    }

    private void OnEventDispatched(object sender, GameEvent e)
    {
        switch (e.EventName)
        {
            case EventQueue.OnStoveStarted:
                audioSource.Play();
                break;
            case EventQueue.OnStoveStopped:
                audioSource.Stop();
                break;
        }
    }
}
