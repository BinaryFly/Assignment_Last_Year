/*
jeroen visser 0952491
*/
using Support;

class Cards
{
    private List<Card> cards;

    public Cards(List<Card> cards)
    {
        this.cards = cards;
    }

    // for now no setter
    // this might be implemented when cards can be added during play to the deck of a player in an expansion
    public List<Card> GetCards { get { return cards; } }

    /// <summary>
    /// Checks if the cards are valid by checking if any cards have more than 3 duplicates in the collection
    /// </summary>
    public bool HasTooManyDuplicates()
    {
        return this.cards
            .GroupBy(card => card.Id)
            .Any(cardSet => cardSet.Count() > 3);
    }

    public bool DeckIsEmpty()
    {
        return GetCardsThatAre<InDeck>().Count() == 0;
    }

    /// <summary>
    /// With this we can get all the cards that have a certain <c>LocationState</c> in the collection
    /// </summary>
    public List<Card> GetCardsThatAre<State>() where State : LocationState
    {
        return this.cards.Where((card) => card.Location is State).ToList();
    }

    /// <summary>
    /// This can be handy to get all the cards that are of a certain cardtype, like getting all lands on board  
    /// </summary>
    public List<CardType> GetCardsThatAre<CardType, State>() where State : LocationState where CardType : Card
    {
        return this.cards.Where((card) => card is CardType && card.Location is State).Select((card) => (CardType)card).ToList();
    }

    public List<CardType> GetCardsThatAre<CardType, ColourType, State>() where CardType : Card where State : LocationState where ColourType : Colour, new()
    {
        var colourToCheck = ColourCreator.GetColour<ColourType>();
        return this.cards.Where((card) =>
        {
            var cardIsType = card is CardType;
            var cardHasState = card.Location is State;
            var costContainsColour = card.Cost.Contains(colourToCheck);
            var isLandAndContainsColour = card is Land && ((Land)card).Colour == colourToCheck;
            return cardIsType && cardHasState && (costContainsColour || isLandAndContainsColour);
        }).Select((card) => (CardType)card).ToList();
    }

    public override string ToString()
    {
        var cardsOnBoard = GetCardsThatAre<OnBoard>().Select((card) => card.Id);
        var onBoardCount = cardsOnBoard.Count();
        var onBoardString = onBoardCount != 0 ? cardsOnBoard.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsInHand = GetCardsThatAre<InHand>().Select((card) => card.Id);
        var inHandCount = cardsInHand.Count();
        var inHandString = inHandCount != 0 ? cardsInHand.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsInDeck = GetCardsThatAre<InDeck>().Select((card) => card.Id);
        var inDeckCount = cardsInDeck.Count();
        var inDeckString = inDeckCount != 0 ? cardsInDeck.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsDisposed = GetCardsThatAre<Disposed>().Select((card) => card.Id);
        var disposedCount = cardsDisposed.Count();
        var disposedString = disposedCount != 0 ? cardsDisposed.Aggregate((acc, next) => acc + ", " + next) : "";

        return @$"On board ({onBoardCount}): {onBoardString}
In hand ({inHandCount}): {inHandString}
In deck ({inDeckCount}): {inDeckString}
Disposed ({disposedCount}): {disposedString}";
        // var orderedCards = this.cards.OrderBy((card) => card.Id);
        // return orderedCards.Select<Card, string>((card) => card.ToString()).Aggregate((acc, next) => acc + "\n\n" + next);
    }
}
