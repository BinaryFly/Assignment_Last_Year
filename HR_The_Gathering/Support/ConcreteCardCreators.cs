/*
jeroen visser 0952491
*/
namespace Support
{
    class NullCardCreator<ColourType> : ImmediateCardCreator where ColourType : Colour, new()
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

    class WaterSpriteCreator : CreatureCardCreator
    {
        protected override string GetId()
        {
            return "water-sprite";
        }

        protected override Colour CreateColour()
        {
            return ColourCreator.GetColour<Blue>();
        }

        protected override List<Effect> CreateEffects()
        {
            return new List<Effect>();
        }

        protected override CardCost CreateEnergyCost()
        {
            return new CardCost(ColourCreator.GetColour<Blue>(), 3);
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
            return "A small watersprite";
        }
    }

    

    class MeadowCreator : LandCardCreator<Green>
    {
        protected override string GetDescription()
        {
            return "a calm and soothing landscape";
        }

        protected override string GetId()
        {
            return "meadow";
        }
    }

    class VolcanoCreator : LandCardCreator<Red>
    {
        protected override string GetDescription()
        {
            return "a fiery and rocky landscape";
        }

        protected override string GetId()
        {
            return "volcano";
        }
    }

    class OceanCreator : LandCardCreator<Blue>
    {
        protected override string GetDescription()
        {
            return "massive body of salt water";
        }

        protected override string GetId()
        {
            return "ocean";
        }
    }

    class DesertCreator : LandCardCreator<Brown>
    {
        protected override string GetDescription()
        {
            return "a landscape of sand and sunburn";
        }

        protected override string GetId()
        {
            return "desert";
        }
    }

    class SkyCreator : LandCardCreator<White>
    {
        protected override string GetDescription()
        {
            return "sky filled with fluffy clouds";
        }

        protected override string GetId()
        {
            return "sky";
        }
    }
}
