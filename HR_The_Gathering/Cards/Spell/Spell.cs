/*
jeroen visser 0952491
*/
// this is just a shell class since it doesn't actually have any differences with a card
// still it might be handy for example when an effect has an effect on all spells in the deck
class Spell : Card
{
    public Spell(string id, string description, CardCost cost, Colour colour) : base(id, description, cost, colour) { }
    public Spell(string id, string description, CardCost cost, Colour colour, List<CardEffect> effects) : base(id, description, cost, colour, effects) { }
}

// this is for spells that are immediately disposed after playing
class Immediate : Spell
{
    public Immediate(Card spell) : base(spell.Id, spell.Description, spell.Cost, spell.Colour, spell.Effects) { }

    public override void Play()
    {
        base.Play();
        this.Location.Dispose();
    }
}

