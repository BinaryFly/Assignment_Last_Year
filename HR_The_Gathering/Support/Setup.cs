/*
jeroen visser 0952491
*/
namespace Support
{

    class CardSetup
    {
        private CardCreator creator;
        private IDictionary<string, int> numberOfCardsInPlace;

        public CardSetup(CardCreator creator, int cardsInDeck, int cardsInHand, int cardsOnBoard, int cardsDisposed)
        {
            this.creator = creator;
            this.numberOfCardsInPlace = new Dictionary<string, int>() {
                { "InDeck", cardsInDeck },
                { "InHand", cardsInHand },
                { "OnBoard", cardsOnBoard },
                { "Disposed", cardsDisposed }
            };
        }

        public IDictionary<string, int> NumberOfCardsInPlace { get => numberOfCardsInPlace; }
        public CardCreator Creator { get => creator; }
        public int TotalCards { get => this.numberOfCardsInPlace.Aggregate<KeyValuePair<string, int>, int>(0, (initialCount, kvPair) => initialCount + kvPair.Value); }

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
            SetupStartingState();
        }

        private static void SetupPlayersAndCards()
        {
            var gb = GameBoard.Instance;
            var playerOne = PlayerOne();
            var playerTwo = PlayerTwo();
            playerOne.Cards = PlayerOneDeck();
            playerTwo.Cards = PlayerTwoDeck();

            // its both players second turn, so for this setup we already make it so the players had their first turn
            // this is also needed because lands can't be turned played on the same turn
            playerOne.IncrementTurn();
            playerTwo.IncrementTurn();

            gb.SetPlayers(playerOne, playerTwo);
        }

        private static Player PlayerOne()
        {
            return new Player("Eric Cartman");
        }

        public static Player PlayerTwo()
        {
            return new Player("Kyle Broflovski");
        }

        private static Cards PlayerOneDeck()
        {
            // has at least 2 lands in his hand in the starting position
            // has at least 1 land in his second turn
            // has at least 1 watersprite in his second turn
            var cardSetups = new List<CardSetup>() {
                new CardSetup(new OceanCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 2, cardsDisposed: 0),
                new CardSetup(new MockColourCard<Blue>(), cardsInDeck: 1, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetup(new WaterSpriteCreator(), cardsInDeck: 0, cardsInHand: 1, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetup(new NullCardCreator<White>(), cardsInDeck: 24, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0)
            };

            var cardList = cardSetups.Aggregate<CardSetup, IEnumerable<Card>>(new List<Card>(),
                    (totalListOfCards, currentCardSetup) => totalListOfCards.Concat(currentCardSetup.GetCards())
            );

            return new Cards(cardList);
        }

        
        private static Cards PlayerTwoDeck()
        {
            var cardSetups = new List<CardSetup>() {
                new CardSetup(new VolcanoCreator(), cardsInDeck: 2, cardsInHand: 0, cardsOnBoard: 1, cardsDisposed: 0),
                new CardSetup(new SkyCreator(), cardsInDeck: 1, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0),
                new CardSetup(new DesertCreator(), cardsInDeck: 1, cardsInHand: 0, cardsOnBoard: 3, cardsDisposed: 0),
                new CardSetup(new NullCardCreator<White>(), cardsInDeck: 25, cardsInHand: 0, cardsOnBoard: 0, cardsDisposed: 0)
            };

            var cardList = cardSetups.Aggregate<CardSetup, IEnumerable<Card>>(new List<Card>(),
                    (totalListOfCards, currentCardSetup) => totalListOfCards.Concat(currentCardSetup.GetCards())
            );

            return new Cards(cardList);
        }


    // after setting the cards for each player set the starting state
    private static void SetupStartingState()
    {
        var gb = GameBoard.Instance;

        // setting up player one
        var playerOneCards = gb.PlayerOne.Cards;

        Dictionary<string, List<string>> playerOneCardsLocation = new Dictionary<string, List<string>>() {
            { "InHand", new List<string>() { "ocean", "water-sprite" } },
            { "OnBoard", new List<string>() { "ocean", "ocean", "ocean" } }
        };

        // setting the cards on board
        foreach (Card card in playerOneCards)
        {
            if (playerOneCardsLocation["OnBoard"].Contains(card.Id))
            {
                card.ChangeLocation(new OnBoard(card));
                playerOneCardsLocation["OnBoard"].Remove(card.Id);
            }
            else if (playerOneCardsLocation["InHand"].Contains(card.Id))
            {
                card.ChangeLocation(new InHand(card));
                playerOneCardsLocation["InHand"].Remove(card.Id);
            }
            /* else if (playerOneCardsLocation["Disposed"].Contains(card.Id)) */
            /* { */
            /*     card.ChangeLocation(new Disposed(card)); */
            /*     playerOneCardsLocation["Disposed"].Remove(card.Id); */
            /* } */
        }

        var playerOneLandsOnBoard = playerOneCards.Lands.OnBoard;
        playerOneLandsOnBoard.Take(2).ToList().ForEach((card) => card.Turn());

        // fill the rest of the hand with null-cards
        foreach (Card card in playerOneCards) if (playerOneCards.InHand.Count() < 7)
            {
                if (card.Id == "null-card")
                {
                    card.ChangeLocation(new InHand(card));
                }
            }

        // turn 2 of the lands on the board
        var playerTwoCards = gb.PlayerTwo.Cards;

        Dictionary<string, List<string>> playerTwoCardsLocation = new Dictionary<string, List<string>>();
        playerTwoCardsLocation.Add("OnBoard", new List<string>() { "volcano", "desert", "desert", "desert" });
        playerTwoCardsLocation.Add("InHand", new List<string>() { "brown-bear", "the-bane-of-my-life" });
        // setting the cards on board
        foreach (Card card in playerTwoCards)
        {
            if (playerTwoCardsLocation["OnBoard"].Contains(card.Id))
            {
                card.ChangeLocation(new OnBoard(card));
                playerTwoCardsLocation["OnBoard"].Remove(card.Id);
            }
            else if (playerTwoCardsLocation["InHand"].Contains(card.Id))
            {
                card.ChangeLocation(new InHand(card));
                playerTwoCardsLocation["InHand"].Remove(card.Id);
            }
            /* else if (playerTwoCardsLocation["Disposed"].Contains(card.Id)) */
            /* { */
            /*     card.ChangeLocation(new Disposed(card)); */
            /*     playerTwoCardsLocation["Disposed"].Remove(card.Id); */
            /* } */
        }

        foreach (Card card in playerTwoCards) if (playerTwoCards.InHand.Count() < 7)
            {
                if (card.Id == "null-card")
                {
                    card.ChangeLocation(new InHand(card));
                }
            }
    }


    // we set all the effects that happen every turn here so they can be overwritten by other effects at runtime
    public static void SetupDefaultEffects()
    {
        EventReactor eventHandler = GameBoard.Instance;

        var logSituationOnNewTurn = new Effect((info) =>
        {
            Console.WriteLine("\n====================");
            Console.WriteLine("New turn is starting...");
            GameBoard.Instance.LogSituation();
        }, Event.LOG_NEW_TURN);
        eventHandler.RegisterEffect(logSituationOnNewTurn);

        // Setting some more custom general events
        var onGameOver = new Effect((_) =>
        {
            var gameboard = GameBoard.Instance;
            gameboard.GameOver();
            gameboard.LogSituation();
        }, Event.PLAYERDIED);
        eventHandler.RegisterEffect(onGameOver);

        // setting a special callback effect for interruptions
        var onInterrupt = new Effect((info) =>
        {
            System.Console.WriteLine(info.Player?.Name);
            var continueInterrupt = false;
            var onYes = () => { continueInterrupt = true; };
            var onNo = () => { };
            var interruptMenu = new YesNoMenu(onYes, onNo);
            interruptMenu.Prompt("Do you wish to interrupt?");
            if (!continueInterrupt) return;

            GameBoard.Instance.Interrupt();
        }, Event.INTERRUPT);
        eventHandler.RegisterEffect(onInterrupt);
    }
}
}
