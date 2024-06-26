/*
jeroen visser 0952491
*/
// some events as an example that they can be hooked into
enum Event
{
    ENTER_PREPARATION_PHASE,
    EXIT_PREPARATION_PHASE,
    ENTER_DRAW_PHASE,
    EXIT_DRAW_PHASE,
    ENTER_ENDING_PHASE,
    EXIT_ENDING_PHASE,
    PLAY_CARD,
    BEFORE_CREATURE_ATTACK,
    AFTER_CREATURE_ATTACK,
    ATTACK,
    PLAYERDIED,
    NEVER,
    INTERRUPT,
    LOG_NEW_TURN
}

// using the reactor pattern to define a reactor for effects and other events defined above in the enum
class EventReactor {
    private IDictionary<Event, IList<Effect>> effects = new Dictionary<Event, IList<Effect>>();
    private bool skipNext = false;

    // this will come in handy when the player died or for some other effect
    protected void UnRegisterAll()
    {
        this.effects = new Dictionary<Event, IList<Effect>>();
    }

    public void RegisterEffect(Effect effect)
    {
        var eventType = effect.Event;
        if (!effects.ContainsKey(eventType))
        {
            effects[eventType] = new List<Effect>();
        }
        effects[eventType].Add(effect);
    }

    public void UnregisterEffect(Effect effect)
    {
        var eventType = effect.Event;
        if (effects.ContainsKey(eventType))
        {
            effects[eventType].Remove(effect);
        }
    }

    public void HandleEvent(Event eventType)
    {
        if (effects.ContainsKey(eventType))
        {
            // creating a deep copy here to prevent adding new effects while the event is being ran
            // also reverse the effects so the execution of the effects is like a Stack (last in, first out)
            var effectsToExecute = effects[eventType].Select(effect => effect).Reverse().ToList();
            foreach (var effect in effectsToExecute)
            {
                if (skipNext) {
                    // possibly remove the effect depending on if it needs to be removed after execution or not
                    effect.Cleanup();
                    this.skipNext = false;
                }

                effect.Execute();
                effect.Cleanup();
            }
        }
    }

    // utility function to handle multiple events at the same time
    // events passed earlier in the list get resolved first
    public void HandleEvents(List<Event> events)
    {
        foreach (Event e in events)
        {
            HandleEvent(e);
        }
    }

    public void SkipNextEffect() 
    {
        // sets a flag to skip the next effect
        this.skipNext = true;
    }
}

