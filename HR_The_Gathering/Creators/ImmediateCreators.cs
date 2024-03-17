/*
jeroen visser 0952491
*/
using Support;

namespace Creators;

// we make a distinction between spells and immediates because immediates are immediately disposed after playing,
// if there are permanent spells they will be created by a SpellCreator rather than by an ImmediateCreator
abstract class ImmediateCreator : SpellCreator
{
    public override Immediate Create()
    {
        return new Immediate(base.Create());
    }
}

// The Cards under here are for mocking some cards in a demo setting or for filling up the deck of a player
class NullCardCreator<ColourType> : ImmediateCreator where ColourType : Colour, new()
{
    protected override CardCost CreateEnergyCost()
    {
        return new NoCost(ColourCreator.GetColour<ColourType>());
    }

    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<ColourType>();
    }

    protected override List<Effect> CreateEffects()
    {
        return new List<Effect>();
    }

    protected override string GetDescription()
    {
        return "this is a dummy card and does nothing";
    }

    protected override string GetId()
    {
        return "null-card";
    }
}

class MockColourCard<ColourType> : NullCardCreator<ColourType> where ColourType : Colour, new()
{
    protected override string GetId()
    {
        var colour = ColourCreator.GetColour<ColourType>();
        return $"{colour.GetType().Name}-mock-card";
    }

    protected override CardCost CreateEnergyCost()
    {
        var energyCost = new CardCost(ColourCreator.GetColour<ColourType>(), 1);
        return energyCost;
    }
}

// The actual concrete cards start under here
class StrengthOfNatureCreator : ImmediateCreator
{
    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<Green>();
    }

    protected override List<Effect> CreateEffects()
    {
        var effects = new List<Effect>();
        effects.Add(new Effect(GiveCreaturePlusFiveAttackAndDefense, Event.PLAY_CARD));
        return effects;
    }

    private void GiveCreaturePlusFiveAttackAndDefense(EffectInfo info)
    {
        if (info.Player is null)
        {
            // for a more professional approach we should throw an error here, instead of just logging the line.
            Console.WriteLine("Incorrect info for StrengthOfNatureCreator");
            return;
        }

        var creaturesOnBoard = info.Player.Cards.Creatures.OnBoard;
        var buffToApply = new Buff(
                (currentAttack) => currentAttack + 5,
                (currentDefense) => currentDefense + 5
            );
        new CardMenu<Creature>(creaturesOnBoard, (creature) =>
        {
            creature.Buff(buffToApply);
        });
    }

    protected override CardCost CreateEnergyCost()
    {
        return new CardCost(ColourCreator.GetColour<Green>(), 1);
    }

    protected override string GetDescription()
    {
        return "Gives +5/+5 to one of your creatures on the board";
    }

    protected override string GetId()
    {
        return "strength-of-nature";
    }
}

class LavaWallCreator : ImmediateCreator
{
    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<Red>();
    }

    protected override List<Effect> CreateEffects()
    {
        var effects = new List<Effect>();
        effects.Add(new Effect(BlockSpell, Event.PLAY_CARD));
        return effects;
    }

    private void BlockSpell(EffectInfo info)  
    {
        GameBoard.Instance.SkipNextEffect();
    }

    protected override CardCost CreateEnergyCost()
    {
        return new CardCost(ColourCreator.GetColour<Red>(), 1);
    }

    protected override string GetDescription()
    {
        return "Block a spell cast by your opponent with the lava wall";
    }

    protected override string GetId()
    {
        return "lava-wall";
    }
}

class AquaShieldCreator : ImmediateCreator
{
    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<Blue>();
    }

    protected override List<Effect> CreateEffects()
    {
        var effects = new List<Effect>();
        effects.Add(new Effect(BlockSpell, Event.PLAY_CARD));
        return effects;
    }

    private void BlockSpell(EffectInfo info)  
    {
        GameBoard.Instance.SkipNextEffect();
    }

    protected override CardCost CreateEnergyCost()
    {
        return new CardCost(ColourCreator.GetColour<Blue>(), 1);
    }

    protected override string GetDescription()
    {
        return "Block a spell cast by your opponent with the aqua shield";
    }

    protected override string GetId()
    {
        return "aqua-shield";
    }
}

