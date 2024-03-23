/*
jeroen visser 0952491
*/
// What has each event / effect in common:
// Type: Permanent / Instantaneous
// Affected: the objects that are affected by the effect itself
// Operation: the operation that is done, think of 'Buff', 'Block', 'Damage' 

abstract class Effect {
    protected Event eventToInvokeOn;
    public abstract void Execute();
    public abstract void Cleanup();

    public Event Event { get => this.eventToInvokeOn; }
}

class DefaultEffect : Effect
{
    protected Action effect;

    public DefaultEffect(Action effectToExecute, Event eventToInvokeOn) 
    {
        this.effect = effectToExecute;
        this.eventToInvokeOn = eventToInvokeOn;
    }

    // The execute method executes some effect based on the action that was passed to it
    public override void Execute()
    {
        this.effect();
    }

    // we don't need to cleanup after default effects, this is more a method for CardEffects
    public override void Cleanup() { }
}

// an effect invoked by a certain Player through playing a card
class CardEffect : Effect
{
    private bool removeAfterOwnerIsDisposed;
    private bool unregisterAfterExecute;
    protected Action<CardEffectInfo> effect;
    private CardEffectInfo info;

    public CardEffect(Action<CardEffectInfo> effectToExecute, Event eventToInvokeOn) : this(effectToExecute, eventToInvokeOn, true, true) { }
    public CardEffect(Action<CardEffectInfo> effectToExecute, Event eventToInvokeOn, bool removeAfterOwnerIsDisposed, bool unregisterAfterExecute)
    {
        this.effect = effectToExecute;
        this.eventToInvokeOn = eventToInvokeOn;
        this.removeAfterOwnerIsDisposed = removeAfterOwnerIsDisposed;
        this.unregisterAfterExecute = unregisterAfterExecute;
        this.info = new CardEffectInfo();
    }

    // unregisterAfterExecute can be set when the effect is created
    // removeAfterOwnerIsDisposed can be set when the effect is created
    // the player and card can't be set until we set the cards on the player

    // The execute method executes some effect based on the action that was passed to it
    public override void Execute()
    {
        this.effect(this.info);
    }

    public override void Cleanup()
    {
        if (this.unregisterAfterExecute)
        {
            GameBoard.Instance.UnregisterEffect(this);
        }
    }

    public void RegisterCard(Card card)
    {
        this.info.Card = card;
    }

    public bool NeedsToBeRemovedAfterCardDispose
    {
        get => this.removeAfterOwnerIsDisposed;
    }

    public CardEffectInfo Info { get => info; set => info = value; }
    // TODO: check if Interrupt is better to implement here on this class instead of the EventReactor
}

// this is used to store the context in which the effect was invoked
// this gets passed to the action whenever it is executed
// we use an object so we can pass more parameters as an object instead of passing the params directly to the action
class CardEffectInfo
{
    Player player;
    Card card;

    internal Card Card { get => card; set => card = value; }
    internal Player Player { get => player; set => player = value; }
}

