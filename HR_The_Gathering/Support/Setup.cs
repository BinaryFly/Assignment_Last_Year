/*
jeroen visser 0952491
*/
using Creators;

namespace Support;

class CardSetupInfo
{
    private CardCreator creator;
    private IDictionary<string, int> numberOfCardsInPlace;

    public CardSetupInfo(CardCreator creator, int cardsInDeck, int cardsInHand, int cardsOnBoard, int cardsDisposed)
    {
        this.creator = creator;
        this.numberOfCardsInPlace = new Dictionary<string, int>() {
                { "InDeck", cardsInDeck },
                { "InHand", cardsInHand },
                { "OnBoard", cardsOnBoard },
                { "Disposed", cardsDisposed }
            };
    }

    public IList<Card> GetCards()
    {
        List<Card> cardsToReturn = new List<Card>();
        foreach (KeyValuePair<string, int> kv in numberOfCardsInPlace)
        {
            for (int i = 0; i < kv.Value; i++)
            {
                var card = creator.Create();
                switch (kv.Key)
                {

                    case "InDeck":
                        card.ChangeLocation(new InDeck(card));
                        break;
                    case "InHand":
                        card.ChangeLocation(new InHand(card));
                        break;
                    case "OnBoard":
                        card.ChangeLocation(new OnBoard(card));
                        break;
                    default:
                        // the card must be of Disposed value here
                        card.ChangeLocation(new Disposed(card));
                        break;
                }
                cardsToReturn.Add(card);
            }
        }

        return cardsToReturn;
    }
}

class Setup
{
    public static void SetupDemoSituation()
    {
        SetupPlayersAndCards();
    }

    private static void SetupPlayersAndCards()
    {
        var gb = GameBoard.Instance;
        var playerOne = PlayerOne();
        var playerTwo = PlayerTwo();
        playerOne.Deck = PlayerOneDeck();
        playerTwo.Deck = PlayerTwoDeck();

        // its both players second turn, so for this setup we already make it so the players had their first turn
        // this is also needed because lands can't be turned played on the same turn
        playerOne.IncrementTurn();
        playerOne.IncrementTurn();
        playerTwo.IncrementTurn();
        playerTwo.IncrementTurn();

        gb.SetPlayers(playerOne, playerTwo);
    }

    private static Player PlayerOne()
    {
        return new Player("Arold");
    }

    public static Player PlayerTwo()
    {
        return new Player("Bryce");
    }

    private static Deck PlayerOneDeck()
    {
        // has at least 2 lands in his hand in the starting position
        // has at least 1 land in his second turn
        // has at least 1 watersprite in his second turn
        var cardSetups = new List<CardSetupInfo>() {
                new CardSetupInfo(new OceanCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 2, cardsDisposed: 0),
                new CardSetupInfo(new MockColourCard<Blue>(), cardsInDeck: 1, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new WaterSpriteCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new AquaShieldCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new NullCardCreator<White>(), cardsInDeck: 21, cardsInHand: 3, cardsOnBoard: 0, cardsDisposed: 0)
            };

        var cardList = cardSetups.Aggregate<CardSetupInfo, IEnumerable<Card>>(new List<Card>(),
                (totalListOfCards, currentCardSetup) => totalListOfCards.Concat(currentCardSetup.GetCards())
        );

        return new Deck(cardList);
    }


    private static Deck PlayerTwoDeck()
    {
        var cardSetups = new List<CardSetupInfo>() {
                new CardSetupInfo(new VolcanoCreator(), cardsInDeck: 2, cardsInHand: 0, cardsOnBoard: 1, cardsDisposed: 0),
                new CardSetupInfo(new SkyCreator(), cardsInDeck: 1, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new DesertCreator(), cardsInDeck: 4, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new LavaWallCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetupInfo(new NullCardCreator<White>(), cardsInDeck: 15, cardsInHand: 6, cardsOnBoard: 0, cardsDisposed: 0)
            };

        var cardList = cardSetups.Aggregate<CardSetupInfo, IEnumerable<Card>>(new List<Card>(),
                (totalListOfCards, currentCardSetup) => totalListOfCards.Concat(currentCardSetup.GetCards())
        );

        return new Deck(cardList);
    }


    // we set all the effects that happen every turn here so they can be overwritten by other effects at runtime
    public static void SetupDefaultEffects()
    {
        EventReactor eventHandler = GameBoard.Instance;

        var logSituationOnNewTurn = new DefaultEffect(() =>
        {
            Console.WriteLine("New turn is starting...");
            Logger.Situation();
        }, Event.LOG_NEW_TURN);
        eventHandler.RegisterEffect(logSituationOnNewTurn);

        // Setting some more custom general events
        var onGameOver = new DefaultEffect(() =>
        {
            var gameboard = GameBoard.Instance;
            gameboard.GameOver();
            Logger.Situation();
        }, Event.PLAYERDIED);
        eventHandler.RegisterEffect(onGameOver);

        // setting a special callback effect for interruptions
        var onInterrupt = new DefaultEffect(() =>
        {
            System.Console.WriteLine("");
            System.Console.WriteLine(GameBoard.Instance.Opponent.Name);
            var continueInterrupt = false;
            var onYes = () => { continueInterrupt = true; };
            var onNo = () => { };
            var interruptMenu = new YesNoMenu(onYes, onNo);
            interruptMenu.Prompt("Do you wish to interrupt?");
            if (!continueInterrupt) return;

            GameBoard.Instance.SwitchPlayers(); // make the opponent the currentplayer temporarily
            GameBoard.Instance.CurrentPlayer.ChooseCardInHandToPlay();
            GameBoard.Instance.SwitchPlayers(); // switch the players back
            
        }, Event.INTERRUPT);
        eventHandler.RegisterEffect(onInterrupt);
    }
}

