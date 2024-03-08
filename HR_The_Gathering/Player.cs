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
    private IDictionary<string, int> generatedEnergy = new Dictionary<string, int>();
    private int turn = 0;

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
    public int Turn { get => turn; }

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
        foreach (Land land in Cards.Lands.OnBoard)
        {
            land.Reset();
        }
        Console.WriteLine("");
    }

    public void ResetCreatures()
    {
        Console.WriteLine("### Creatures are being reset to defending state ###");
        foreach (Creature creature in Cards.Creatures.OnBoard)
        {
            creature.PerformDefend();
        }
        Console.WriteLine("");
    }

    public void DrawCard()
    {
        // checking if there are any cards left to draw
        // if not send a 'PlayerDiedEvent' to the eventHandler
        var cardToDraw = this.Cards.InDeck.FirstOrDefault();

        if (cardToDraw is null)
        {
            Console.WriteLine($"No more cards to draw, killing player {this.Name}");
            this.IsDead = true;
            return;
        }

        Console.WriteLine("### A card is being drawn... ###");
        // drawing the actual card
        cardToDraw.Draw();
        Console.WriteLine("");
    }


    // for now we just discard the cards that are at the end of the hand
    // a better implementation would be to let the player choose which cards to dispose
    public void DiscardExcessCards()
    {
        Console.WriteLine("### Disposing excess cards ###");
        var cardsInHand = this.Cards.InHand;
        var cardsToDispose = cardsInHand.Count() - Constants.MAX_CARDS_IN_HAND;

        for (int cardsDisposed = 0; cardsDisposed < cardsToDispose; cardsDisposed++)
        {
            cardsInHand.Last().Dispose();
        }

        Console.WriteLine("");
    }

    public void ChooseCardInHandToPlay()
    {
        var cardsInHand = Cards.InHand;
        var cardMenu = new CardMenu<Card>(cardsInHand, PlaySelectedCard);
        cardMenu.Prompt("Which card do you want to play?:");
    }

    private void PlaySelectedCard(Card card)
    {
        var chosenLands = ChooseLandsToTurn(card.Cost.Amount);
        TurnLands(chosenLands);
        if (card.Cost.CanBePaidWith(generatedEnergy))
        {
            card.Cost.Pay(generatedEnergy);
            card.Play();
        }
        else
        {
            // we reset the lands that were turned before and remove the energy in the generated energy
            Console.WriteLine("Not enough energy to play card!");
            chosenLands.ForEach((land) => land.Reset());
            generatedEnergy.Clear();
        }

    }

    // TODO: test this
    private List<Land> ChooseLandsToTurn(int amountOfLandsNeeded)
    {
        if (amountOfLandsNeeded == 0)
        {
            // return early without nominating lands, since none need to be turned
            return new List<Land>();
        }

        // the lands that are able to turn are all the lands that are UnTurned and not played this turn
        var landsAbleToTurn = Cards.Lands.OnBoard.Where((land) =>
                land.State is UnTurned && land.TurnPlayed != this.Turn).ToList();
        var chosenLands = new List<Land>();
        if (landsAbleToTurn.Count < amountOfLandsNeeded)
        {
            Console.WriteLine("Not enough lands available to satisfy cost...");
            return chosenLands;
        }

        while (chosenLands.Count < amountOfLandsNeeded)
        {
            // we have to keep making a new menu in the while loop here to update the query
            var chooseLandMenu = new CardMenu<Land>(landsAbleToTurn, (land) =>
            {
                landsAbleToTurn.Remove(land);
                chosenLands.Add(land);
            });
            chooseLandMenu.Prompt($"Please choose a land to turn (nominated lands: {chosenLands.Count} of {amountOfLandsNeeded})");
        }
        return chosenLands;
    }

    private void TurnLands(List<Land> lands)
    {
        lands.ForEach((land) =>
        {
            var landEnergy = land.Turn();
            if (landEnergy is null)
            {
                Console.WriteLine("Land can't generate energy at the moment");
                return;
            }

            // the TryAdd here is to make sure the energy is added to the dictionary if its not already in there
            if (!this.generatedEnergy.ContainsKey($"{landEnergy}"))
            {
                this.generatedEnergy.Add($"{landEnergy}", 0);
            }
            this.generatedEnergy[$"{landEnergy}"]++;
        });
    }

    public List<Creature> GetDefendingCreatures()
    {
        return this.Cards.Creatures.OnBoard.Where((creature) => creature.State is Defending).ToList();
    }

    public void IncrementTurn()
    {
        this.turn++;
    }
}
