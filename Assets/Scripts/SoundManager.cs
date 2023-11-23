using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] chop;
    [SerializeField] private AudioClip[] deliverySuccessSound;
    [SerializeField] private AudioClip[] deliveryFailureSound;
    [SerializeField] private AudioClip[] objectDrop;
    [SerializeField] private AudioClip[] objectPickup;
    [SerializeField] private AudioClip[] trash;
    [SerializeField] private AudioClip[] warning;

    [Header("Configuration")]
    [SerializeField] private float volume = 1f;

    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    private void OnEnable()
    {
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
            case EventQueue.OnDeliverySuccess:
                PlaySoundInSelection(deliverySuccessSound, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnDeliveryFailure:
                PlaySoundInSelection(deliveryFailureSound, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnIngredientChop:
                PlaySoundInSelection(chop, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnObjectDrop:
                PlaySoundInSelection(objectDrop, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnObjectPickup:
                PlaySoundInSelection(objectPickup, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnItemTrashed:
                PlaySoundInSelection(trash, e.Origin.transform.position, volume);
                break;

            case EventQueue.OnStoveBurned:
            case EventQueue.OnStoveCooked:
                PlaySoundInSelection(warning, e.Origin.transform.position, volume);
                break;
        }
    }

    private void PlaySoundInSelection(AudioClip[] audioClips, Vector3 position, float volume = 1f)
    {
        var audioClip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

}
