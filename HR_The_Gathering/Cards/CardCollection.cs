/*
jeroen visser 0952491
*/
using System.Collections;
using System.Text;
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
// there are only a few methods that need to be added for the base deck that we will use, other than that its just a cardcollection
class Deck : CardCollection<Card>
{
    public Deck(IEnumerable<Card> cards) : base(cards) { }

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
        var finalString = new StringBuilder();

        var cardsByLocation = this.GroupBy((card) => card.Location.GetType().ToString());
        var locationStrings = cardsByLocation.Select((group) =>
        {
            var cardIds = group.Select((card) => card.Id);
            var cardCount = cardIds.Count();
            var cardString = cardCount != 0 ? cardIds.Aggregate((acc, next) => acc + ", " + next) : "";

            string formatString;
            switch (group.Key)
            {
                case "InDeck":
                    formatString = $"In deck ({cardCount}): {cardString}";
                    break;
                case "InHand":
                    formatString = $"In hand ({cardCount}): {cardString}";
                    break;
                case "OnBoard":
                    formatString = $"On board ({cardCount}): {cardString}";
                    break;
                default:
                    // this must be Disposed
                    formatString = $"Disposed ({cardCount}): {cardString}";
                    break;
            }

            return MakeStringWidthMax(Constants.MENU_WIDTH, formatString);

        });
        

        // TODO: if players don't have a certain LocationState in their cards then it won't show up in this string either
        return String.Join("\n", locationStrings);
    }

    // helps with the above ToString method, this will make it so every string is not wider than a certain given maxLength
    private string MakeStringWidthMax(int maxLength, string stringToModify)
    {
        string[] words = stringToModify.Split(' ');
        StringBuilder sb = new StringBuilder();
        sb.Append("|");
        int newLines = 0; // keeps track of the newlines in the string, these will be deducted of the currLength since we don't want them as the length
        int currLength = 1;
        foreach (string word in words)
        {
            if (currLength + word.Length + 1 < maxLength)
            {
                sb.AppendFormat(" {0}", word);
            }
            else
            {
                sb.AppendFormat("{0," + (maxLength - currLength) + "}{1}", "|", Environment.NewLine);
                sb.AppendFormat("| {0}", word);
                newLines++;
            }
            currLength = ((sb.Length - newLines) % maxLength);
        }

        // currLength is without the space after the string
        sb.AppendFormat("{0," + (maxLength - currLength) + "}", "|");

        return sb.ToString();
    }
}
