/*
jeroen visser 0952491
*/
using PatternUtils;

abstract class CreatureState : State<Creature>
{
    protected CreatureState(Creature creature) : base(creature) { }

    public virtual void Attack(Target target) { }

    public virtual void Defend() { }
}

class Attacking : CreatureState
{
    public Attacking(Creature creature) : base(creature) { }

    public override void Defend()
    {
        Console.WriteLine($"Setting creature state to defending: {this.owner.ToString()}");
        this.owner.State = new Defending(this.owner);
    }
}

// whenever an attack by the opponent is happening we check if there are creatures with this state
// if so then we don't show the player as a valid target but the targets that can be chosen for the attack will be
// any creatures on the board
class Defending : CreatureState
{
    public Defending(Creature creature) : base(creature) { }

    public override void Attack(Target target)
    {
        var attackValue = this.owner.Attack;
        Console.WriteLine($"\"{this.owner.GetName()}\" deals {attackValue} damage to \"{target.GetName()}\"");
        target.TakeDamage(attackValue);
        Console.WriteLine($"Setting creature state to attacking: {this.owner.ToString()}");
        this.owner.State = new Attacking(this.owner);
    }
}

// creature is inactive at the moment it gets disposed
class Inactive : CreatureState
{
    public Inactive(Creature creature) : base(creature) { }
}
