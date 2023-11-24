using UnityEngine;

public class PlayerWalkingSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;

    [Header("Configuration")]
    [SerializeField] private float stepIntervalInSeconds = 0.25f;

    private bool? wasWalking = null;
    private float timeSinceLastStep = 0f;

    void Update()
    {
        if (player.IsWalking)
        {
            if (wasWalking == true)
            {
                wasWalking = true;
                PlayStepSound();
            }

            timeSinceLastStep += Time.deltaTime;
            if (timeSinceLastStep >= stepIntervalInSeconds)
            {
                timeSinceLastStep = 0f;

                if (!audioSource.isPlaying)
                {
                    PlayStepSound();
                }
            }
        }
        else
        {
            wasWalking = false;
            timeSinceLastStep = 0f;
        }
    }

    private void PlayStepSound()
    {
        var audioClip = this.stepSounds[Random.Range(0, this.stepSounds.Length)];

        this.audioSource.clip = audioClip;
        this.audioSource.Play();
    }
}
