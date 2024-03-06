/*
jeroen visser 0952491
*/
abstract class Card
{
    private LocationState locationState;
    private string identifier; // if two cards are the same they should have the same identifier
    private string description;
    private EnergyCost cost;
    private List<Effect> effects = new List<Effect> { };

    public Card(string id, string description, EnergyCost cost) {
        this.identifier = id;
        this.locationState = new InDeck(this);
        this.description = description;
        this.cost = cost;
    }
    public Card(string id, string description, EnergyCost cost, List<Effect> effects) : this(id, description, cost)
    {
        this.effects = effects;
    }

    public LocationState Location { get => locationState; }
    public string Id { get => identifier; }
    public virtual string Description { get => description; }
    public EnergyCost Cost { get => cost; }
    public List<Effect> Effects { get => effects; }

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

    public virtual void Retrieve()
    {
        this.Location.Retrieve();
    }

    public override string ToString()
    {
        return $"{Id} -- {Description}";
    }

    public void ChangeLocation(LocationState location)
    {
        this.locationState = location;
    }
}

class Creature : Card, Target
{
    private CreatureState creatureState;
    private int attack;
    private int defense;
    private List<Buff> buffs = new List<Buff>();
    private List<Damage> damages = new List<Damage>();

    public Creature(string id, string description, EnergyCost cost, int attack, int defense)
        : this(id, description, cost, new List<Effect>(), attack, defense) { }

    public Creature(string id, string description, EnergyCost cost, List<Effect> effects, int attack, int defense)
        : base(id, description, cost, effects)
    {
        this.attack = attack;
        this.defense = defense;
        this.creatureState = new Defending(this);
    }

    public CreatureState State
    {
        get { return creatureState; }
        set { this.creatureState = value; }
    }

    public int Attack
    {
        get => this.buffs.Aggregate(attack, (att, buff) => buff.Attack(att));
    }

    public int Defense
    {
        get
        {
            var allDefenses = this.buffs.Concat(damages);
            return allDefenses.Aggregate(defense, (def, buff) => buff.Defense(def));
        }
    }

    public override string Description { get => $"({this.Attack}/{this.Defense}) {base.Description}"; }

    public string GetName()
    {
        return $"{this.Id} -- {this.Description}";
    }

    public void TakeDamage(int damage)
    {
        this.damages.Add(new Damage(damage));
        this.DisposeIfDead();
    }

    public void PerformDefend()
    {
        this.State.Defend();
    }

    public void PerformAttack(Target target)
    {
        GameBoard.Instance.HandleEvents(new List<Event>() { Event.BEFORE_CREATURE_ATTACK, Event.ATTACK });
        this.State.Attack(target);
        GameBoard.Instance.HandleEvent(Event.AFTER_CREATURE_ATTACK);
    }

    public void Buff(Buff buff)
    {
        this.buffs.Add(buff);
        // maybe in practice a buff can be negative?
        this.DisposeIfDead();
    }

    public void RemoveBuff(Buff buff)
    {
        this.buffs.Remove(buff);
        this.DisposeIfDead();
    }

    private void DisposeIfDead()
    {
        if (this.Defense <= 0)
        {
            this.Location.Dispose();
            this.State = new Inactive(this);
        }
    }

    // this removes any damages and buffs from the creature
    public void ResetToInitial()
    {
        this.buffs = new List<Buff>();
        this.damages = new List<Damage>();
        this.State = new Defending(this);
    }
}

// helper class to manage transformations of creatures
class Buff
{
    private Func<int, int> modifyAttack = (attack) => attack;
    private Func<int, int> modifyDefense = (defense) => defense;

    public Buff(Func<int, int> modifyAttack, Func<int, int> modifyDefense)
    {
        this.modifyAttack = modifyAttack;
        this.modifyDefense = modifyDefense;
    }

    public int Attack(int attack) => modifyAttack(attack);
    public int Defense(int defense) => modifyDefense(defense);
}

// make a special "damage" buff, that will be able to see the buffed defenses and subtract from them when calculating total defense
class Damage : Buff
{
    public Damage(int damage) : base((att) => att, (def) => def - damage) { }
}


class Land : Card
{
    private LandState landState;
    private Colour colour;

    // we use the passed colour here to set the cost, this is purerly out of convenience
    // we do not actually use the cost field if a land get's played since we will check if it's type is a land
    // before checking the cost of the card
    public Land(string id, string description, Colour colour) : base(id, description, new NoCost())
    {
        this.landState = new UnTurned(this);
        this.colour = colour;
    }

    public LandState State
    {
        get { return landState; }
        set { this.landState = value; }
    }

    public Colour Colour { get => colour; }

    public override string Description { get => $"({this.landState.GetType().Name}) {base.Description}"; }
    public void Reset()
    {
        this.State.Reset();
    }

    public void Turn()
    {
        this.State.Turn();
    }

    public Colour? GetEnergy()
    {
        return this.State.GetEnergy();
    }
}

// this is just a shell class since it doesn't actually have any differences with a card
// still it might be handy for example when an effect has an effect on all spells in the deck
class Spell : Card
{
    public Spell(string id, string description, EnergyCost cost) : base(id, description, cost) { }
    public Spell(string id, string description, EnergyCost cost, List<Effect> effects) : base(id, description, cost, effects) { }
}

// this is for spells that are immediately disposed after playing
class Immediate : Spell
{
    public Immediate(Card spell) : base(spell.Id, spell.Description, spell.Cost, spell.Effects) { }

    public override void Play()
    {
        base.Play();
        this.Location.Dispose();
    }
}

