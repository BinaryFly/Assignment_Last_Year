/*
jeroen visser 0952491
*/
namespace Support
{
    // a creator for cards using the template method pattern
    // giving colour type as a generic parameter to satisfy Colours for Lands
    abstract class CardCreator
    {
        public abstract Card Create();

        protected abstract string GetId();
        protected abstract string GetDescription();
        protected abstract EnergyCost CreateEnergyCost();
        protected abstract List<Effect> CreateEffects();
    }

    abstract class SpellCardCreator : CardCreator
    {
        public override Spell Create()
        {
            var id = this.GetId();
            var description = this.GetDescription();
            var cost = this.CreateEnergyCost();
            var effects = this.CreateEffects();
            return new Spell(id, description, cost, effects);
        }
    }

    abstract class ImmediateCardCreator : SpellCardCreator
    {
        public override Immediate Create()
        {
            return new Immediate(base.Create());
        }
    }

    // the new constraint here specifies that we can instantiate the colourtype with new() without parameters
    abstract class LandCardCreator<LandColour> : CardCreator where LandColour : Colour, new()
    {

        public override Land Create()
        {
            var id = this.GetId();
            var description = this.GetDescription();
            var colour = this.CreateColour();
            return new Land(id, description, colour);
        }

        protected Colour CreateColour()
        {
            return ColourCreator.GetColour<LandColour>();
        }

        // methods under here will never get invoked but otherwise the compiler will complain that we 
        // don't override all abstract inherited members
        protected override NoCost CreateEnergyCost()
        {
            return new NoCost();
        }

        protected override List<Effect> CreateEffects()
        {
            return new List<Effect> { };
        }
    }

    abstract class CreatureCardCreator : CardCreator
    {
        public override Creature Create()
        {
            var id = this.GetId();
            var description = this.GetDescription();
            var cost = this.CreateEnergyCost();
            var effects = this.CreateEffects();
            var attack = this.GetAttack();
            var defense = this.GetDefense();
            return new Creature(id, description, cost, effects, attack, defense);
        }

        abstract protected int GetAttack();
        abstract protected int GetDefense();
    }

    // sort of a flyweight inpired pattern
    class ColourCreator
    {
        private static List<Colour> colours = new List<Colour>();

        // the new constraint here specifies that we can instantiate the colourtype with new() without parameters
        public static Colour GetColour<ColourType>() where ColourType : Colour, new()
        {
            var foundColours = colours.Where((colour) => colour.GetType() is ColourType);
            if (foundColours.Count() == 1)
            {
                return foundColours.ToList()[0];
            }
            var colourToReturn = new ColourType();
            ColourCreator.colours.Add(colourToReturn);
            return colourToReturn;
        }
    }
}
