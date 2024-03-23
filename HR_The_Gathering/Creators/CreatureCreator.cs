/*
jeroen visser 0952491
*/
namespace Creators;

abstract class CreatureCreator : CardCreator
{
    public override Creature Create()
    {
        var id = this.GetId();
        var description = this.GetDescription();
        var cost = this.CreateEnergyCost();
        var colour = this.CreateColour();
        var effects = this.CreateEffects();
        var attack = this.GetAttack();
        var defense = this.GetDefense();
        var createdCard = new Creature(id, description, cost, colour, effects, attack, defense);
        return createdCard;
    }

    abstract protected int GetAttack();
    abstract protected int GetDefense();
}

class WaterSpriteCreator : CreatureCreator
{
    protected override string GetId()
    {
        return "water-sprite";
    }

    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<Blue>();
    }

    protected override List<CardEffect> CreateEffects()
    {
        var effects = new List<CardEffect>();
        effects.Add(new CardEffect(this.DisposeCardFromOpponentsHandEffect, Event.PLAY_CARD));
        return effects;
    }

    private void DisposeCardFromOpponentsHandEffect(CardEffectInfo info)
    {
        var opponentCards = GameBoard.Instance.Opponent.Deck.InHand;
        var indexOfCardToDispose = new Random().Next() % opponentCards.Count();
        var cardToDispose = opponentCards.ToList()[indexOfCardToDispose];
        cardToDispose.Dispose();
    }

    protected override CardCost CreateEnergyCost()
    {
        return new CardCost(ColourCreator.GetColour<Blue>(), 2);
    }

    protected override int GetAttack()
    {
        return 2;
    }

    protected override int GetDefense()
    {
        return 2;
    }

    protected override string GetDescription()
    {
        return "A small watersprite, removes a random card from your opponents hand when it comes into play";
    }
}
