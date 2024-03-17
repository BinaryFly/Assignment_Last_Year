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
