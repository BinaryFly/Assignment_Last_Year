/*
jeroen visser 0952491
*/
using System.Collections;
using Support;

class CardCollection<CardType> : IEnumerable<CardType> where CardType : Card
{
    private List<CardType> cards;

    public CardCollection(IEnumerable<CardType> cards)
    {
        this.cards = cards.ToList();
    }

    public List<CardType> Cards { get => cards; }

    // these properties allow for nice access to certain types of cards, you could, for example, do the following:
    // this.Cards.InDeck.Creatures.Red to get all red creatures in the deck, this should work with any other type of combination
    public CardCollection<CardType> InDeck { get => new CardCollection<CardType>(this.GetCardsThatAre<InDeck>()); } 
    public CardCollection<CardType> OnBoard { get => new CardCollection<CardType>(this.GetCardsThatAre<OnBoard>()); } 
    public CardCollection<CardType> InHand { get => new CardCollection<CardType>(this.GetCardsThatAre<InHand>()); } 
    public CardCollection<CardType> Disposed { get => new CardCollection<CardType>(this.GetCardsThatAre<Disposed>()); } 

    public CardCollection<Creature> Creatures { get => new CardCollection<Creature>(GetCardsOfType<Creature>()); } 
    public CardCollection<Spell> Spells { get => new CardCollection<Spell>(GetCardsOfType<Spell>()); } 
    public CardCollection<Land> Lands { get => new CardCollection<Land>(GetCardsOfType<Land>()); } 

    public CardCollection<Card> Red { get => new CardCollection<Card>(GetCardsWithColour<Red>()); } 
    public CardCollection<Card> Blue { get => new CardCollection<Card>(GetCardsWithColour<Blue>()); } 
    public CardCollection<Card> Green { get => new CardCollection<Card>(GetCardsWithColour<Green>()); } 
    public CardCollection<Card> Brown { get => new CardCollection<Card>(GetCardsWithColour<Brown>()); } 
    public CardCollection<Card> White { get => new CardCollection<Card>(GetCardsWithColour<White>()); } 

    public IEnumerator<CardType> GetEnumerator()
    {
        for (int i = 0; i < this.Cards.Count(); i++)
        {
            yield return this.Cards[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < this.Cards.Count(); i++)
        {
            yield return this.Cards[i];
        }
    }
    
    // to get all card types with a certain locationstate
    private IEnumerable<CardType> GetCardsThatAre<State>() where State : LocationState
    {
        return this.Where((card) => card.Location is State).ToList();
    }

    // to get all cards of a certain type
    private IEnumerable<T> GetCardsOfType<T>() where T : Card
    {
        return this.OfType<T>();
    }

    // to get all cards of a certain colour
    // we return the IEnumerable with overarching generic type 'Card' here since we can't determine if we only return one cardType because of the way we set colours on lands
    private IEnumerable<Card> GetCardsWithColour<ColourType>() where ColourType : Colour, new()
    {
        var colourToGet = ColourCreator.GetColour<ColourType>();
        return this.Where((card) => card.Colour == colourToGet);
    }
}


// this is the class for the base deck of the player
class Cards : CardCollection<Card>
{
    public Cards(IEnumerable<Card> cards) : base(cards) { }

    /// <summary>
    /// Checks if the cards are valid by checking if any cards have more than 3 duplicates in the collection
    /// </summary>
    public bool HasTooManyDuplicates()
    {
        return this.Cards
            .GroupBy(card => card.Id)
            .Any(cardSet => cardSet.Count() > 3);
    }

    public bool DeckIsEmpty()
    {
        return this.InDeck.Count() == 0;
    }

    public override string ToString()
    {
        var cardsOnBoard = this.OnBoard.Select((card) => card.Id);
        var onBoardCount = cardsOnBoard.Count();
        var onBoardString = onBoardCount != 0 ? cardsOnBoard.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsInHand = this.InHand.Select((card) => card.Id);
        var inHandCount = cardsInHand.Count();
        var inHandString = inHandCount != 0 ? cardsInHand.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsInDeck = this.InDeck.Select((card) => card.Id);
        var inDeckCount = cardsInDeck.Count();
        var inDeckString = inDeckCount != 0 ? cardsInDeck.Aggregate((acc, next) => acc + ", " + next) : "";

        var cardsDisposed = this.Disposed.Select((card) => card.Id);
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
