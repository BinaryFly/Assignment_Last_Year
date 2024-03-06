/*
jeroen visser 0952491
*/
namespace Support
{
    class NullCardCreator : ImmediateCardCreator
    {
        protected override EnergyCost CreateEnergyCost()
        {
            return new NoCost();
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

    class MockColourCard<ColourType> : NullCardCreator where ColourType : Colour, new()
    {
        protected override string GetId()
        {
            var colour = ColourCreator.GetColour<ColourType>();
            return $"{colour.GetType().Name}-mock-card";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var energyCost = new EnergyCost();
            energyCost.Add<ColourType>();
            return energyCost;
        }
    }


    class ZupparColdTrapCreator : SpellCardCreator
    {
        protected override string GetId()
        {
            return "zuppar-cold-trap";
        }

        protected override string GetDescription()
        {
            return "When activated, this blows an icy wind over all the creatures that can attack";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var zupparColdTrapCost = new EnergyCost();
            for (int i = 0; i < 3; i++)
            {
                zupparColdTrapCost.Add<Blue>();
            }
            return zupparColdTrapCost;
        }

        protected override List<Effect> CreateEffects()
        {

            List<Effect> effects = new List<Effect> { };

            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;

            effects.Add(ZupparColdTrapEffect(effectInfo));
            return effects;
        }

        private Effect ZupparColdTrapEffect(EffectInfo effectInfo)
        {

            var zupparColdTrapEffect = new Effect((info) =>
            {
                var gb = GameBoard.Instance;

                // this effect should be only invoked on the opponents turn
                if (gb.CurrentPlayer == info.Player)
                {
                    return;
                }

                Console.WriteLine("The effect of zuppar cold trap is invoked");
                var creaturesOnBoard = gb.CurrentPlayer.Cards.GetCardsThatAre<Creature, OnBoard>();
                // get all creatures that are able to attack from the board
                // if the creatures are defending they are still able to attack
                creaturesOnBoard.Where((creature) => creature.State is Defending && creature.Attack > 0);
                foreach (Creature creature in creaturesOnBoard)
                {
                    creature.TakeDamage(2);
                }
                info.Card?.Dispose();
            }, Event.ATTACK, effectInfo);
            return zupparColdTrapEffect;
        }
    }

    class HiddenDangerCreator : SpellCardCreator
    {
        protected override string GetId()
        {
            return "hidden-danger";
        }

        protected override string GetDescription()
        {
            return "beware the danger that is hidden...";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var hiddenDangerCost = new EnergyCost();
            for (int i = 0; i < 4; i++)
            {
                hiddenDangerCost.Add<Red>();
            }
            return hiddenDangerCost;
        }

        protected override List<Effect> CreateEffects()
        {
            List<Effect> effects = new List<Effect> { };

            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;

            effects.Add(HiddenDangerEffect(effectInfo));
            effects.Add(SkipNextDrawingPhase());
            return effects;
        }

        private Effect HiddenDangerEffect(EffectInfo effectInfo)
        {
            var hiddenDangerEffect = new Effect((info) =>
            {
                // only invoke and remove this if the current player is the opponent of the 
                // player that registered this effect
                var gb = GameBoard.Instance;
                if (gb.CurrentPlayer == info.Player)
                {
                    return;
                }

                Console.WriteLine("The effect of hidden danger is invoked");
                var allCreaturesOnBoard = GetAllCreaturesOnBoard(gb);
                DealDamageToCreatures(allCreaturesOnBoard, 4);

                info.Card?.Dispose();
            }, Event.ENTER_PREPARATION_PHASE, effectInfo);
            return hiddenDangerEffect;
        }

        private List<Creature> GetAllCreaturesOnBoard(GameBoard gb)
        {
            var playerOneCreatures = gb.PlayerOne.Cards.GetCardsThatAre<Creature, OnBoard>();
            var playerTwoCreatures = gb.PlayerTwo.Cards.GetCardsThatAre<Creature, OnBoard>();
            return playerOneCreatures.Concat(playerTwoCreatures).ToList();
        }

        private void DealDamageToCreatures(List<Creature> creatures, int damage)
        {
            foreach (Creature creature in creatures)
            {
                creature.TakeDamage(damage);
            }
        }

        private Effect SkipNextDrawingPhase()
        {
            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;
            effectInfo.RemoveAfterOwnerIsDisposed = false;
            var skipNextDrawingPhase = new Effect((info) =>
            {
                Console.WriteLine("Skipping drawing phase");
                GameBoard.Instance.UpdatePhase(new Main());
            }, Event.EXIT_PREPARATION_PHASE, effectInfo);
            return skipNextDrawingPhase;
        }
    }

    class KnownGameCreator : SpellCardCreator
    {
        protected override string GetId()
        {
            return "known-game";
        }

        protected override string GetDescription()
        {
            return "4 damage is dealt to all the creatures on the board that are able to attack";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var knownGameCost = new EnergyCost();
            for (int i = 0; i < 4; i++)
            {
                knownGameCost.Add<Blue>();
            }
            return knownGameCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();

            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;

            effects.Add(KnownGameEffect(effectInfo));

            return effects;
        }

        private Effect KnownGameEffect(EffectInfo effectInfo)
        {

            var knownGameEffect = new Effect((info) =>
            {
                var gb = GameBoard.Instance;

                // this effect should be only invoked on the opponents turn
                if (gb.CurrentPlayer == info.Player)
                {
                    return;
                }

                Console.WriteLine("The effect of known game is invoked");
                // this effect can be interrupted by the player
                gb.HandleEvent(Event.INTERRUPT);
                var creaturesOnBoard = gb.CurrentPlayer.Cards.GetCardsThatAre<Creature, OnBoard>();
                // get all creatures that are able to attack from the board
                // if the creatures are defending they are still able to attack
                creaturesOnBoard.Where((creature) => creature.State is Defending && creature.Attack > 0);
                foreach (Creature creature in creaturesOnBoard)
                {
                    creature.TakeDamage(4);
                }
                info.Card?.Dispose();
            }, Event.BEFORE_CREATURE_ATTACK, effectInfo);
            return knownGameEffect;
        }
    }

    class WarriorPotionCreator : ImmediateCardCreator
    {
        protected override string GetId()
        {
            return "warrior-potion";
        }

        protected override string GetDescription()
        {
            return "a temporary strengthening potion";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var warriorPotionCost = new EnergyCost();
            for (int i = 0; i < 3; i++)
            {
                warriorPotionCost.Add<Red>();
            }
            return warriorPotionCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            var effectInfo = new EffectInfo();

            // to determine when the effect will be unregistered
            effectInfo.UnregisterAfterExecute = true;
            effectInfo.RemoveAfterOwnerIsDisposed = false;

            effects.Add(WarriorPotionEffect(effectInfo));
            return effects;
        }

        private Effect WarriorPotionEffect(EffectInfo effectInfo)
        {
            var warriorPotionEffect = new Effect((info) =>
            {
                var currentPlayerCreatures = info.Player?.Cards.GetCardsThatAre<Creature, OnBoard>();
                var cardMenu = new CardMenu<Creature>(currentPlayerCreatures!, (creature) =>
                {
                    var warriorPotionBuff = new Buff(att => att + 5, def => def + 3);
                    creature.Buff(warriorPotionBuff);
                    Console.WriteLine($"New stats of {creature.Id}: ({creature.Attack}/{creature.Defense})");

                    // to maintain a reference to the chosen creature
                    // we create the callback to undo the effect inside
                    // the creating effect
                    RegisterRemoveBuff(creature, warriorPotionBuff);
                });
                cardMenu.Prompt("Which creature do you want to give the warrior potion?");
            }, Event.PLAY_CARD, effectInfo);
            return warriorPotionEffect;
        }

        private void RegisterRemoveBuff(Creature creature, Buff buff)
        {
            var removeBuff = new Effect((info) =>
            {
                creature.RemoveBuff(buff);
            }, Event.ENTER_ENDING_PHASE);
            GameBoard.Instance.RegisterEffect(removeBuff);
        }
    }

    class BrokenStaffOfMerlinCreator : SpellCardCreator
    {
        protected override string GetId()
        {
            return "broken-staff-of-merlin";
        }

        protected override string GetDescription()
        {
            return "merlins broken staff, it still has some lingering effects around it...";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var brokenStaffOfMerlinCost = new EnergyCost();
            brokenStaffOfMerlinCost.Add<Green>();
            brokenStaffOfMerlinCost.Add<Blue>();
            return brokenStaffOfMerlinCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            var buff = new Buff((attack) => (int)Math.Ceiling(attack / 2.0), (defense) => defense);

            var firstEffectInvocation = true;
            //  with this effect we create one effect 
            //  for handling lowering attack on the turn of whoever plays this
            //  and we create an effect for the user 
            var halveAttackOfOwnedCreatures = new Effect((info) =>
            {
                // create a new effect info to ensure the temporary buff effects are removed after execution
                var newEffectInfo = new EffectInfo();
                newEffectInfo.Card = info.Card;
                newEffectInfo.Player = info.Player;
                newEffectInfo.UnregisterAfterExecute = true;

                // this is set on the first run of this effect
                // this is because it can be set in the middle of the turn
                // and doesn't necessarily follow an event
                if (info.Player == GameBoard.Instance.CurrentPlayer && firstEffectInvocation)
                {
                    var playerCreatures = GetCreaturesFrom(info.Player);
                    SetBuffOnCreatures(playerCreatures, buff);
                    RegisterRemoveBuffs(playerCreatures, buff, newEffectInfo);
                    firstEffectInvocation = false;
                }

                // this is for subsequent turns
                // if somehow the preparation phase of the owner is skipped
                // and this effect still lives on
                var ownerEffect = new Effect((info) =>
                {
                    if (info.Player == GameBoard.Instance.CurrentPlayer)
                    {
                        var playerCreatures = GetCreaturesFrom(info.Player);
                        SetBuffOnCreatures(playerCreatures, buff);
                        RegisterRemoveBuffs(playerCreatures, buff, newEffectInfo);
                    }
                }, Event.ENTER_DRAW_PHASE, newEffectInfo);

                // also set an effect for the opponent creatures
                var opponentEffect = new Effect((info) =>
                {
                    // don't invoke this when the current player is the
                    // player that set the effect
                    if (info.Player == GameBoard.Instance.CurrentPlayer) return;

                    var opponentCreatures = GetCreaturesFrom(GameBoard.Instance.CurrentPlayer);
                    SetBuffOnCreatures(opponentCreatures, buff);
                    RegisterRemoveBuffs(opponentCreatures, buff, newEffectInfo);
                }, Event.ENTER_PREPARATION_PHASE, newEffectInfo);

                GameBoard.Instance.RegisterEffect(ownerEffect);
                GameBoard.Instance.RegisterEffect(opponentEffect);

            }, Event.PLAY_CARD);

            effects.Add(halveAttackOfOwnedCreatures);
            effects.Add(SkipDrawingPhase());
            effects.Add(DestroyOnOwnerPreparationPhase());
            return effects;
        }


        // helper methods for creating effects
        private List<Creature> GetCreaturesFrom(Player player)
        {
            return player.Cards.GetCardsThatAre<Creature, OnBoard>();
        }

        private void SetBuffOnCreatures(List<Creature> creatures, Buff buff)
        {
            creatures.ForEach((creature) => creature.Buff(buff));
        }

        private void RegisterRemoveBuffs(List<Creature> creatures, Buff buff, EffectInfo effectInfo)
        {
            var removeBuffs = new Effect((info) =>
            {
                creatures.ForEach((creature) => creature.RemoveBuff(buff));
            }, Event.ENTER_ENDING_PHASE, effectInfo);
            GameBoard.Instance.RegisterEffect(removeBuffs);
        }

        private Effect SkipDrawingPhase()
        {
            var skipNextDrawingPhase = new Effect((info) =>
            {
                Console.WriteLine("Skipping drawing phase");
                GameBoard.Instance.UpdatePhase(new Main());
            }, Event.EXIT_PREPARATION_PHASE);
            return skipNextDrawingPhase;
        }

        private Effect DestroyOnOwnerPreparationPhase()
        {
            var destroyOnOwnerPreparationPhase = new Effect((info) =>
            {
                if (info.Player == GameBoard.Instance.CurrentPlayer)
                {
                    info.Card?.Dispose();
                }
            }, Event.ENTER_PREPARATION_PHASE);
            return destroyOnOwnerPreparationPhase;
        }
    }

    class GetBackPetCreator : SpellCardCreator
    {
        protected override string GetId()
        {
            return "get-back-pet";
        }

        protected override string GetDescription()
        {
            return "Summon a brown card to your hand from the disposed pile";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var energyCost = new EnergyCost();
            energyCost.Add<Brown>();
            return energyCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;
            var effects = new List<Effect>();
            effects.Add(GetBackPetEffect(effectInfo));
            return effects;
        }

        private Effect GetBackPetEffect(EffectInfo effectInfo)
        {
            var getBackPet = new Effect((info) =>
            {
                var cardsToChooseFrom = info.Player?.Cards.GetCardsThatAre<Creature, Brown, Disposed>();
                if (cardsToChooseFrom is not null && cardsToChooseFrom.Count > 0)
                {
                    var cardMenu = new CardMenu<Creature>(cardsToChooseFrom, (card) =>
                    {
                        card.Retrieve();
                    });

                    cardMenu.Prompt("WHich card do you want to return to your hand?");
                    info.Card?.Dispose();
                    return;
                }
                Console.WriteLine("No cards to get back from the disposed pile...");
                // this sets the location back into the hand if the effect can't be invoked
                // TODO: the only thing would still be that the land can't be unturned
                info.Card?.ChangeLocation(new InHand(info.Card));
            }, Event.PLAY_CARD, effectInfo);
            return getBackPet;
        }
    }

    class BaneOfMyLifeCreator : ImmediateCardCreator
    {
        protected override string GetId()
        {
            return "the-bane-of-my-life";
        }

        protected override string GetDescription()
        {
            return "deals 5 damage to any target";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var energyCost = new EnergyCost();
            for (int i = 0; i < 5; i++)
            {
                energyCost.Add<Brown>();
            }
            return energyCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            effects.Add(BaneOfMyLifeEffect());
            return effects;

        }

        private Effect BaneOfMyLifeEffect()
        {
            var baneOfMyLifeEffect = new Effect((info) =>
            {
                var targets = GameBoard.Instance.GetTargets();
                var attackMenu = new TargetMenu(targets, (target) => { target.TakeDamage(5); });
                attackMenu.Prompt("Choose a target to attack");
            }, Event.PLAY_CARD);
            return baneOfMyLifeEffect;
        }
    }

    class FireStrikeCreator : ImmediateCardCreator
    {
        protected override string GetId()
        {
            return "fire-strike";
        }

        protected override string GetDescription()
        {
            return "deals fire damage to the chosen target";
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var firestrikeCost = new EnergyCost();
            firestrikeCost.Add<Red>();
            return firestrikeCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;

            effects.Add(FireStrikeEffect(effectInfo));
            return effects;
        }

        private Effect FireStrikeEffect(EffectInfo effectInfo)
        {
            var firestrikEffect = new Effect((info) =>
            {
                var targets = GameBoard.Instance.GetTargets();
                var attackMenu = new TargetMenu(targets, (target) =>
                {
                    GameBoard.Instance.HandleEvent(Event.ATTACK);
                    target.TakeDamage(5);
                });
                attackMenu.Prompt("Choose who you wish to attack...");
            }, Event.PLAY_CARD, effectInfo);
            return firestrikEffect;
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

    class LavaWormCreator : CreatureCardCreator
    {
        protected override EnergyCost CreateEnergyCost()
        {
            var lavaWormCost = new EnergyCost();
            lavaWormCost.Add<Red>();
            return lavaWormCost;
        }

        protected override List<Effect> CreateEffects()
        {
            return new List<Effect>();
        }

        protected override string GetDescription()
        {
            return "this worm can leave some nasty skin burns";
        }

        protected override string GetId()
        {
            return "lava-worm";
        }

        protected override int GetAttack()
        {
            return 2;
        }

        protected override int GetDefense()
        {
            return 2;
        }
    }

    class FireImpCreator : CreatureCardCreator
    {
        protected override EnergyCost CreateEnergyCost()
        {
            var fireImpCost = new EnergyCost();
            fireImpCost.Add<Red>();
            fireImpCost.Add<Red>();
            return fireImpCost;
        }

        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;
            effects.Add(FireImpEffect(effectInfo));
            return effects;
        }

        private Effect FireImpEffect(EffectInfo effectInfo)
        {
            var fireImpEffect = new Effect((info) =>
            {
                var opponent = GameBoard.Instance.Opponent;
                var opponentCardsInHand = opponent.Cards.GetCardsThatAre<InHand>();
                // TODO: this should actually pick a random card from the opponents hand
                //  to avoid complexity we just remove the first card from the opponents hand
                if (opponentCardsInHand.Count == 0)
                {
                    Console.WriteLine("Fire imp had no cards to dispose...");
                    return;
                }

                Console.WriteLine("Fire imp is disposing card...");
                opponentCardsInHand.Last().Dispose();
            }, Event.PLAY_CARD, effectInfo);
            return fireImpEffect;
        }

        protected override string GetDescription()
        {
            return "a mischievous imp born in the flames of mt.lava";
        }

        protected override string GetId()
        {
            return "fire-imp";
        }

        protected override int GetAttack()
        {
            return 2;
        }
        protected override int GetDefense()
        {
            return 2;
        }
    }

    class WaterSpriteCreator : CreatureCardCreator
    {
        protected override List<Effect> CreateEffects()
        {
            return new List<Effect>();
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var energyCost = new EnergyCost();
            energyCost.Add<Blue>();
            return energyCost;
        }

        protected override string GetDescription()
        {
            return "A small water sprite";
        }

        protected override string GetId()
        {
            return "water-sprite";
        }

        protected override int GetAttack()
        {
            return 1;
        }

        protected override int GetDefense()
        {
            return 2;
        }

    }

    class BrownBearCreator : CreatureCardCreator
    {
        protected override List<Effect> CreateEffects()
        {
            var effects = new List<Effect>();
            var effectInfo = new EffectInfo();
            effectInfo.UnregisterAfterExecute = true;
            effects.Add(BrownBearEffect(effectInfo));
            return effects;
        }

        private Effect BrownBearEffect(EffectInfo effectInfo)
        {
            var brownBearEffect = new Effect((info) =>
            {
                GameBoard.Instance.Opponent.TakeDamage(1);
            }, Event.PLAY_CARD, effectInfo);
            return brownBearEffect;
        }

        protected override EnergyCost CreateEnergyCost()
        {
            var energyCost = new EnergyCost();
            energyCost.Add<Brown>();
            for (int i = 0; i < 3; i++)
            {
                energyCost.Add<White>();
            }
            return energyCost;
        }

        protected override string GetDescription()
        {
            return "A fierce bear that lashes out immediately once summoned";
        }

        protected override string GetId()
        {
            return "brown-bear";
        }

        protected override int GetAttack()
        {
            return 5;
        }

        protected override int GetDefense()
        {
            return 1;
        }
    }
}
