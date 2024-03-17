/*
jeroen visser 0952491
*/
using PatternUtils;

abstract class LocationState : State<Card>
{
    protected LocationState(Card card) : base(card) { }


    public virtual void Draw()
    {
        // only valid if state is InDeck
        throw new MethodAccessException();
    }

    public virtual void Play()
    {
        // energy should be checked on a higher level before this can be called
        // only works when the card is InHand, if so set the state to OnBoard
        // generate an effect depending on the context of the State (owner), only works if the cardstate is InHand
        throw new MethodAccessException();
    }

    public virtual void Dispose()
    {
        // set the state to Disposed, does only work if the card is OnBoard
        throw new MethodAccessException();
    }

    public virtual void Retrieve()
    {
        throw new MethodAccessException();
    }
}


class InDeck : LocationState
{
    public InDeck(Card card) : base(card) { }

    public override void Draw()
    {
        Console.WriteLine($"Drawing card: {this.owner.ToString()}");
        this.owner.ChangeLocation(new InHand(this.owner));
    }
}

class InHand : LocationState
{
    public InHand(Card card) : base(card) { }

    public override void Play()
    {
        Console.WriteLine($"Playing card: {this.owner.ToString()}");
        this.owner.TurnPlayed = GameBoard.Instance.CurrentPlayer.Turn;
        // every card goes on the board, immediate spells get disposed right after anyway
        this.owner.ChangeLocation(new OnBoard(this.owner));
        // registering all effects here in case a card has more than one effect
        this.owner.Effects.ForEach((effect) =>
        {
            effect.Card = this.owner;
            effect.Player = GameBoard.Instance.CurrentPlayer;
            GameBoard.Instance.RegisterEffect(effect);
        });

        // We ask the opponent here if they want to interrupt us playing the card
        // the card is only interruptible if its not a land
        if (this.owner is not Land) {
            GameBoard.Instance.HandleEvent(Event.INTERRUPT);
        }

        GameBoard.Instance.HandleEvent(Event.PLAY_CARD);
    }

    // can also dispose cards if there are too many in hand
    public override void Dispose()
    {
        Console.WriteLine($"Disposing card: {this.owner.ToString()}");
        this.owner.ChangeLocation(new Disposed(this.owner));
        // remove any leftover effects that were invoked by this card after it gets disposed
        this.owner.Effects.ForEach(effect =>
        {
            if (effect.NeedsToBeRemovedAfterCardDispose)
            {
                GameBoard.Instance.UnregisterEffect(effect);
            }
        });
    }
}

class OnBoard : LocationState
{
    public OnBoard(Card card) : base(card) { }

    public override void Dispose()
    {
        Console.WriteLine($"Disposing card: {this.owner.ToString()}");
        this.owner.ChangeLocation(new Disposed(this.owner));
        // remove any leftover effects that were invoked by this card after it gets disposed
        this.owner.Effects.ForEach(effect =>
        {
            if (effect.NeedsToBeRemovedAfterCardDispose)
            {
                GameBoard.Instance.UnregisterEffect(effect);
            }
        });
    }
}

class Disposed : LocationState
{
    public Disposed(Card card) : base(card) { }

    public override void Retrieve()
    {
        Console.WriteLine($"Retrieving card: {this.owner.ToString()}");
        this.owner.ChangeLocation(new InHand(this.owner));
        if (this.owner is Creature)
        {
            var creature = ((Creature)this.owner);
            creature.ResetToInitial();
        }
    }
}

