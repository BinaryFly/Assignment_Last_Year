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
            var effects = new List<Effect>();
            effects.Add(new Effect(this.DisposeCardFromOpponentsHandEffect, Event.PLAY_CARD));
            return effects;
        }

        private void DisposeCardFromOpponentsHandEffect(EffectInfo info)
        {
            var opponentCards = GameBoard.Instance.Opponent.Cards.InHand;
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

    class StrengthOfNatureCreator : ImmediateCardCreator
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

    class LavaWallCreator : ImmediateCardCreator
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

    class AquaShield : ImmediateCardCreator
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
