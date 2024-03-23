/*
jeroen visser 0952491
*/
namespace Creators;

abstract class SpellCreator : CardCreator
{
    public override Spell Create()
    {
        var id = this.GetId();
        var description = this.GetDescription();
        var cost = this.CreateEnergyCost();
        var colour = this.CreateColour();
        var effects = this.CreateEffects();
        var createdCard = new Spell(id, description, cost, colour, effects);
        return createdCard;
    }
}
