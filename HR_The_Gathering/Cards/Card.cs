/*
jeroen visser 0952491
*/
abstract class Card
{
    private LocationState locationState;
    private string identifier; // if two cards are the same they should have the same identifier
    private string description; 
    private CardCost cost;
    private List<CardEffect> effects = new List<CardEffect> { };
    private Colour colour;

    public Card(string id, string description, CardCost cost, Colour colour) {
        this.identifier = id;
        this.locationState = new InDeck(this);
        this.description = description;
        this.cost = cost;
        this.colour = colour;
    }

    public Card(string id, string description, CardCost cost, Colour colour, List<CardEffect> effects) : this(id, description, cost, colour)
    {
        this.effects = effects;

        // we make sure the cards is registered with the effect it invokes
        // this is handy for the CardEffectInfo passed to the invoked action by the effect
        foreach (CardEffect effect in this.effects)
        {
            effect.RegisterCard(this);
        }
    }

    public LocationState Location { get => locationState; }
    public string Id { get => identifier; }
    public virtual string Description { get => description; }
    public CardCost Cost { get => cost; }
    public List<CardEffect> Effects { get => effects; }
    public Colour Colour { get => colour; }

    // we implement all the methods of the location state 
    // under the card so we can wrap this behaviour with a different
    // implementation in derived classes
    public virtual void Play()
    {
        this.Location.Play();
    }

    public virtual void Draw()
    {
        this.Location.Draw();
    }

    public virtual void Dispose()
    {
        this.Location.Dispose();
    }

    public override string ToString()
    {
        return $"{Id} -- {Description}";
    }

    // is mainly used to change the location in setup functions 
    // without activating any effects in the meanwhile
    public void ChangeLocation(LocationState location)
    {
        this.locationState = location;
    }
}



