/*
jeroen visser 0952491
*/
class Land : Card
{
    private LandState landState;

    // we use the passed colour here to set the cost, this is purerly out of convenience
    // we do not actually use the cost field if a land get's played since we will check if it's type is a land
    // before checking the cost of the card
    public Land(string id, string description, Colour colour) : base(id, description, new NoCost(colour), colour)
    {
        this.landState = new Turned(this);
    }

    public LandState State
    {
        get { return landState; }
        set { this.landState = value; }
    }

    public override string Description { get => $"({this.landState.GetType().Name}) {base.Description}"; }
    public void Reset()
    {
        this.State.Reset();
    }

    // gives back a Colour/Energy depending on if the card has already been turned yet
    public Colour? Turn()
    {
        return this.State.Turn();
    }
}
