using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventQueue", menuName = "KitchenChaos/EventQueue", order = 1)]
public class EventQueue : ScriptableObject
{
    public const string OnIngredientChop = "INGREDIENT_CHOP";
    public const string OnDeliverySuccess = "DELIVERY_SUCCESS";
    public const string OnDeliveryFailure = "DELIVERY_FAILURE";
    public const string OnFootstep = "FOOTSTEP";
    public const string OnObjectDrop = "OBJECT_DROP";
    public const string OnObjectPickup = "OBJECT_PICKUP";
    public const string OnStoveStarted = "STOVE_STARTED";
    public const string OnStoveStopped = "STOVE_STOPPED";
    public const string OnStoveCooked = "STOVE_COOKED";
    public const string OnStoveBurned = "STOVE_BURNED";
    public const string OnItemTrashed = "ITEM_TRASHED";

    public const string OnGameStateChanged = "GAME_STATE_CHANGED";
    public const string OnGameTimerTicked = "GAME_TIMER_TICKED";

    public event EventHandler<GameEvent> OnEventDispatched;

    public void DispatchEvent(string eventName, GameObject origin, object extraData = null)
    {
        OnEventDispatched?.Invoke(this, new GameEvent { EventName = eventName, Origin = origin, ExtraData = extraData });
    }
}

public class GameEvent : EventArgs
{
    public string EventName { get; set; }

    public GameObject Origin { get; set; }

    public object ExtraData { get; set; }
}