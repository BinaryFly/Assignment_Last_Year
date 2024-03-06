/*
jeroen visser 0952491
*/
// implementing the basics for player, later this might need to be integrated with a specific design pattern
using Support;

// this is also implemented on creatures
// this is so players and creatures have a combined type for attackers
interface Target
{
    string GetName();
    void TakeDamage(int damage);
}


class Player : Target
{
    private int health = 10;
    private string name;
    private bool dead = false;
    private Cards cards = new Cards(new List<Card>());
    private EnergyPayment generatedEnergy = new EnergyPayment();

    public Player(string name)
    {
        this.name = name;
    }

    public Cards Cards { get => cards; set => cards = value; }

    public int Health
    {
        get => health;
        private set
        {
            health = value;
            if (value <= 0)
            {
                this.IsDead = true;
            }
        }
    }

    public bool IsDead
    {
        get => this.dead;
        private set
        {
            this.dead = value;
            if (value)
            {
                GameBoard.Instance.HandleEvent(Event.PLAYERDIED);
            }
        }
    }

    public string Name { get => this.name; }

    // yes this is the same as the property above, but this is implemented to satisfy the Target interface
    public string GetName()
    {
        return this.Name;
    }

    public void TakeDamage(int damage)
    {
        this.Health -= damage;
    }

    public void ResetLands()
    {
        Console.WriteLine("### Lands are being reset ###");
        var landsOnBoard = Cards.GetCardsThatAre<Land, OnBoard>();
        landsOnBoard.ForEach((land) => land.Reset());
        Console.WriteLine("");
    }

    public void ResetCreatures()
    {
        Console.WriteLine("### Creatures are being reset to defending state ###");
        var creaturesOnBoard = Cards.GetCardsThatAre<Creature, OnBoard>();
        creaturesOnBoard.ForEach((creature) => creature.PerformDefend());
        Console.WriteLine("");
    }

    public void DrawCard()
    {
        // checking if there are any cards left to draw
        // if not send a 'PlayerDiedEvent' to the eventHandler
        var cardsInDeck = this.Cards.GetCardsThatAre<InDeck>();

        if (cardsInDeck.Count() == 0)
        {
            Console.WriteLine($"No more cards to draw, killing player {this.Name}");
            this.IsDead = true;
            return;
        }

        Console.WriteLine("### A card is being drawn... ###");
        // drawing the actual card
        var cardToDraw = cardsInDeck[0];
        cardToDraw.Draw();
        Console.WriteLine("");
    }


    // for now we just discard the cards that are at the end of the hand
    // a better implementation would be to let the player choose which cards to dispose
    public void DiscardExcessCards()
    {
        Console.WriteLine("### Disposing excess cards ###");
        var cardsInHand = this.Cards.GetCardsThatAre<InHand>();
        var cardsToDispose = cardsInHand.Count() - Constants.MAX_CARDS_IN_HAND;

        for (int cardsDisposed = 0; cardsDisposed < cardsToDispose; cardsDisposed++)
        {
            cardsInHand.Last().Dispose();
        }

        Console.WriteLine("");
    }

    public void ChooseCardInHandToPlay()
    {
        var cardsInHand = Cards.GetCardsThatAre<InHand>();
        var cardMenu = new CardMenu<Card>(cardsInHand, PlaySelectedCard);
        cardMenu.Prompt("Which card do you want to play?:");
    }

    private void PlaySelectedCard(Card card)
    {
        if (card is Land)
        {
            card.Play();
            return;
        }

        PlayCardWithLandEnergy(card);
    }

    private void PlayCardWithLandEnergy(Card card)
    {
        var numberOfColours = card.Cost.ToList().Count;
        var landsToTurn = ChooseLandsToTurn(numberOfColours);
        if (!generatedEnergy.CanPayFor(card.Cost))
        {
            Console.WriteLine("Not enough energy to play card!");
            this.generatedEnergy.Clear();
            return;
        }
        landsToTurn.ForEach((land) => land.State.Turn());
        card.Play();
    }

    private List<Land> ChooseLandsToTurn(int amountOfLandsNeeded)
    {
        var landsOnBoard = Cards.GetCardsThatAre<Land, OnBoard>();
        var landsAbleToTurn = landsOnBoard.Where((land) => land.State is UnTurned).ToList();
        var nominatedLands = new List<Land>();
        if (landsAbleToTurn.Count < amountOfLandsNeeded)
        {
            Console.WriteLine("Not enough lands available to satisfy cost...");
            return nominatedLands;
        }

        while (nominatedLands.Count < amountOfLandsNeeded)
        {
            var chooseLandMenu = new CardMenu<Land>(landsAbleToTurn, (land) =>
            {
                // we modify the state of the list here that is passed to the menu,
                // allowing the menu to have one less option, 
                // when adding functionality to menu this might become unsafe
                landsAbleToTurn.Remove(land);
                var landEnergy = land.GetEnergy();
                if (landEnergy is not null)
                {
                    this.generatedEnergy.Add(landEnergy);
                    nominatedLands.Add(land);
                }
                else
                {
                    Console.WriteLine("Land can't generate energy at the moment");
                }
            });
            chooseLandMenu.Prompt($"Please choose a land to use (nominated lands: {nominatedLands.Count} of {amountOfLandsNeeded})");
        }

        return nominatedLands;
    }

    public List<Creature> GetDefendingCreatures()
    {
        var creaturesOnBoard = this.Cards.GetCardsThatAre<Creature, OnBoard>();
        return creaturesOnBoard.Where((creature) => creature.State is Defending).ToList();
    }
}
