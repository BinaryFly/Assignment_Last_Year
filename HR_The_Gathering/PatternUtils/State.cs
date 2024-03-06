/*
jeroen visser 0952491
*/
namespace PatternUtils
{
    /// <summary>
    /// The <c>State</c> class represents a state that can be owned by some owner of the state
    /// the owner will also provide the context of the state used in any implemented methods later
    /// </summary>
    abstract class State<OwnerType>
    {
        protected OwnerType owner;

        protected State(OwnerType owner)
        {
            this.owner = owner;
        }
    }
}

