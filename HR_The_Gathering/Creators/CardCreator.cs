/*
jeroen visser 0952491
*/
namespace Creators;

// a creator for cards using the template method pattern
// giving colour type as a generic parameter to satisfy Colours for Lands
abstract class CardCreator
{
    public abstract Card Create();

    protected abstract string GetId();
    protected abstract string GetDescription();
    protected abstract CardCost CreateEnergyCost();
    protected abstract Colour CreateColour();
    protected abstract List<Effect> CreateEffects();
}

