/*
jeroen visser 0952491
*/
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
