using System;
using System.Collections.Generic;
using UnityEngine;

public class Fryable : MonoBehaviour, IIngredient
{
    public event EventHandler<FryableUpdatedEventArgs> FryableUpdated;

    public class FryableUpdatedEventArgs : EventArgs
    {
        public FryState State { get; set; }

        public float CurrentProgress { get; set; }
        public float MaxProgress { get; set; }
    }

    public enum FryState
    {
        Raw,
        Cooked,
        Burned
    };

    private static readonly IReadOnlyDictionary<FryState, float> progressTimes = new Dictionary<FryState, float>()
    {
        { FryState.Raw, 5f },
        { FryState.Cooked, 7f }
    };

    [SerializeField]
    private Transform raw;

    [SerializeField]
    private Transform cooked;

    [SerializeField]
    private Transform burned;

    private float timeCooked = 0f;

    private bool isCooking = false;

    private FryState state = FryState.Raw;
    public FryState State
    {
        get
        {
            return state;
        }
        private set
        {
            state = value;

            switch (state)
            {
                case FryState.Raw:
                    raw.gameObject.SetActive(true);
                    cooked.gameObject.SetActive(false);
                    burned.gameObject.SetActive(false);
                    break;
                case FryState.Cooked:
                    raw.gameObject.SetActive(false);
                    cooked.gameObject.SetActive(true);
                    burned.gameObject.SetActive(false);
                    break;
                case FryState.Burned:
                    raw.gameObject.SetActive(false);
                    cooked.gameObject.SetActive(false);
                    burned.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void StartCooking()
    {
        isCooking = true;
    }

    public void StopCooking()
    {
        isCooking = false;
    }

    public float GetCookingProgress()
    {
        if (State == FryState.Burned)
        {
            return 1f;
        }

        var maxTime = progressTimes[State];
        return this.timeCooked / maxTime;
    }

    private void Update()
    {
        if (!isCooking || State == FryState.Burned)
        {
            return;
        }

        var maxTime = progressTimes[State];
        timeCooked += Time.deltaTime;

        if (timeCooked > maxTime)
        {
            // We progress to the next state
            State = State switch
            {
                FryState.Raw => FryState.Cooked,
                FryState.Cooked => FryState.Burned,
                _ => FryState.Burned,
            };

            timeCooked = 0f;
        }

        FryableUpdated?.Invoke(this, new FryableUpdatedEventArgs()
        {
            State = State,
            CurrentProgress = timeCooked,
            MaxProgress = maxTime
        });
    }

    public bool CanBePlated()
    {
        return State != FryState.Raw;
    }

    public bool IsDesirable()
    {
        return State == FryState.Cooked;
    }
}
