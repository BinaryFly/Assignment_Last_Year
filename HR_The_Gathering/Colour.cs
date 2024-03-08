/*
jeroen visser 0952491
*/
class CardCost
{
    private Colour colour;
    private int cost;

    // the colours passed here is the cost, this can consist of multiple different colours
    public CardCost(Colour colour, int cost)
    {
        this.colour = colour;
        this.cost = cost;
    }

    public int Amount { get => cost; }

    // we get an IEnumerable here because you can pay with mutliple different colours of Energy
    public virtual bool CanBePaidWith(IDictionary<string, int> colours)
    {
        // assign the minimumvalue to the out value, then try to get it
        int paymentValue = int.MinValue;
        colours.TryGetValue($"{this.colour}", out paymentValue);
        return paymentValue >= this.cost;
    }

    public virtual void Pay(IDictionary<string, int> colours)
    {
        // remove the cost from the given payment
        if (colours.ContainsKey($"{this.colour}")) {
            colours[$"{this.colour}"] -= this.cost;
        }
        else {
            Console.WriteLine($"Did not find key {this.colour} in card");
        }
    }
}

class NoCost : CardCost
{
    // We pass the colour and a cost of 0 to create a NoCost object
    public NoCost(Colour colour) : base(colour, 0) { }

    public override bool CanBePaidWith(IDictionary<string, int> colours)
    {
        // this will always be true since there is no cost
        return true;
    }

    public override void Pay(IDictionary<string, int> colours)
    {
        Console.WriteLine("No need to pay for cards with no cost");
    }
}


abstract class Colour
{
    private string name;

    public Colour(string name)
    {
        this.name = name;
    }

    // can be overridden later when 
    public virtual bool Equals(Colour colour)
    {
        if (ReferenceEquals(colour, null))
        {
            return false;
        }

        return (ReferenceEquals(this, colour) || this.name == colour.name);
    }
}

class Blue : Colour
{
    public Blue() : base("Blue") { }
}

class Red : Colour
{
    public Red() : base("Red") { }
}

class Brown : Colour
{
    public Brown() : base("Brown") { }
}

class White : Colour
{
    public White() : base("White") { }
}

class Green : Colour
{
    public Green() : base("Green") { }
}
