/*
jeroen visser 0952491
*/
using PatternUtils;
abstract class LandState : State<Land>
{
    protected LandState(Land land) : base(land) { }

    public virtual Colour? Turn() { return null; }
    public virtual void Reset() { }
}

class Turned : LandState
{
    public Turned(Land land) : base(land) { }

    public override void Reset()
    {
        Console.WriteLine($"Resetting land: {this.owner.Id}");
        this.owner.State = new UnTurned(this.owner);
    }
}

class UnTurned : LandState
{
    public UnTurned(Land land) : base(land) { }

    public override Colour? Turn()
    {
        // return null if it is the same turn as the land was played
        if (this.owner.TurnPlayed == GameBoard.Instance.CurrentPlayer.Turn) 
        {
            return null;
        }

        Console.WriteLine($"Turning land: {this.owner.Id}");
        this.owner.State = new Turned(this.owner);
        return this.owner.Colour;
    }
}
