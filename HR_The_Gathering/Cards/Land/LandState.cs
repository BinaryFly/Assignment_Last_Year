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

// a land always starts out Turned, since we need to wait a turn after playing before we can turn it
// lands on board are always reset during the preparation phase
// and since we play this land during the main phase we can just give it an initial 'Turned' state
// to ensure it can't be turned until the next turn.
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
        Console.WriteLine($"Turning land: {this.owner.Id}");
        this.owner.State = new Turned(this.owner);
        return this.owner.Colour;
    }
}
