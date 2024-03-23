/*
jeroen visser 0952491
*/
class Creature : Card, Target
{
    private CreatureState creatureState;
    private int attack;
    private int defense;
    private List<Buff> buffs = new List<Buff>();
    private List<Damage> damages = new List<Damage>();

    // creatures with no activation of permanent effect
    public Creature(string id, string description, CardCost cost, Colour colour, int attack, int defense)
        : this(id, description, cost, colour, new List<CardEffect>(), attack, defense) { }

    // for creatures with actual effects
    public Creature(string id, string description, CardCost cost, Colour colour, List<CardEffect> effects, int attack, int defense)
        : base(id, description, cost, colour, effects)
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
        // reduces the attack by using Aggregate (reduce) to put all buffs on the initial attack before attacking
        get => this.buffs.Aggregate(attack, (att, buff) => buff.Attack(att));
    }

    public int Defense
    {
        get
        {
            // concatenate the damages with the buffs for calculating the defense
            var allDefenses = this.buffs.Concat(damages);

            // reduces the defense by using Aggregate (reduce) to put all buffs on the initial defense before attacking
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
