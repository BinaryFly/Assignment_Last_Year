/*
jeroen visser 0952491
*/
using Support;

interface EnergyCollection
{
    void Add(Colour energyToAdd);
    List<Colour> ToList();
}

class EnergyCost : EnergyCollection
{
    private List<Colour> colours = new List<Colour>();

    public void Add(Colour colourToAdd)
    {
        this.colours.Add(colourToAdd);
    }

    public void Add<ColourType>() where ColourType : Colour, new()
    {
        this.Add(ColourCreator.GetColour<ColourType>());
    }

    public virtual List<Colour> ToList()
    {
        // flattens all stored colours into a list, 
        // with Select we return a deep copy of the list
        return this.colours.Select(c => c).ToList();
    }

    public bool Contains(Colour colour)
    {
        return this.colours.Contains(colour);
    }
}

class NoCost : EnergyCost
{
    public override List<Colour> ToList()
    {
        return new List<Colour>();
    }
}

class EnergyPayment : EnergyCollection
{
    private List<Colour> energy = new List<Colour>();

    public void Add(Colour energyLike)
    {
        energy.Add(energyLike);
    }

    public List<Colour> ToList()
    {
       return this.energy.Select(e => e).ToList(); 
    }

    public bool CanPayFor(EnergyCost energyCost)
    {
        // making a deep copy with select
        var costCopy = energyCost.ToList();
        var paymentCopy = this.energy.Select(e => e).ToList();

        var virtuallyPaidFor = new List<Colour>();

        // this compares all the colours and removes potentially paid ones from the cost 
        foreach (Colour paymentUnit in paymentCopy)
        {
            // find if the energy can be found in the cost
            var foundElement = costCopy.Find((costUnit) => paymentUnit.Equals(costUnit));

            if (foundElement is not null)
            {
                // remove colour from the list to avoid equality for the same object
                // then add the colour to a new list so we can compare the payment and the satisfied colours at the end of this method
                costCopy.Remove(foundElement);
                virtuallyPaidFor.Add(foundElement);
                continue;
            }
            else 
            {
                Console.WriteLine($"Payment {foundElement} is extra");
                // TODO: maybe exit here or something, or let know that there is more payment than cost or that the two are not even
            }
        }

        return virtuallyPaidFor.Count == paymentCopy.Count;
    }

    public void Clear()
    {
        this.energy = new List<Colour>();
    }
}

abstract class Colour
{
    private string name;

    public Colour(string name)
    {
        this.name = name;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Colour)
        {
            return false;
        }

        return Equals(obj);
    }

    // need to override GetHashCode for Equals override convention
    public override int GetHashCode()
    {
        return base.GetHashCode();
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
