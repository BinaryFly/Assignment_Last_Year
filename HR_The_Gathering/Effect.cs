/*
jeroen visser 0952491
*/
// What has each event / effect in common:
// Type: Permanent / Instantaneous
// Affected: the objects that are affected by the effect itself
// Operation: the operation that is done, think of 'Buff', 'Block', 'Damage' 
class Effect
{
    protected Action<EffectInfo> effect;
    private EffectInfo info;
    protected Event eventToInvokeOn;

    public Effect(Action<EffectInfo> effectToExecute, Event eventToInvokeOn) : this(effectToExecute, eventToInvokeOn, new EffectInfo()) { }
    public Effect(Action<EffectInfo> effectToExecute, Event eventToInvokeOn, EffectInfo info)
    {
        this.effect = effectToExecute;
        this.info = info;
        this.eventToInvokeOn = eventToInvokeOn;
    }

    // The execute method executes some effect based on the action that was passed to it
    public void Execute()
    {
        this.effect(info);
    }

    public void Cleanup()
    {
        if (info.UnregisterAfterExecute)
        {
            GameBoard.Instance.UnregisterEffect(this);
        }
    }

    public Event Event { get => eventToInvokeOn; }
    public Player? Player { get => info.Player; set => info.Player = value; }
    public Card? Card { get => info.Card; set => info.Card = value; }
    public bool NeedsToBeRemovedAfterCardDispose
    {
        get => info.RemoveAfterOwnerIsDisposed;
    }
}

// this is used to store the context in which the effect was invoked
// this gets passed to the action whenever it is executed
class EffectInfo
{
    // the card and player that own this effect
    private Card? card;
    private Player? player;
    private bool unregisterAfterExecute = false;
    private bool removeAfterOwnerIsDisposed = true;

    public Player? Player { get => player; set => player = value; }
    public Card? Card { get => card; set => card = value; }
    public bool UnregisterAfterExecute { get => unregisterAfterExecute; set => unregisterAfterExecute = value; }
    public bool RemoveAfterOwnerIsDisposed { get => removeAfterOwnerIsDisposed; set => removeAfterOwnerIsDisposed = value; }
}

