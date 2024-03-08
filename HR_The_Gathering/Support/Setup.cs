/*
jeroen visser 0952491
*/
namespace Support
{
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
            gb.PlayerOne = PlayerOne();
            gb.PlayerTwo = PlayerTwo();
            gb.PlayerOne.Cards = PlayerOneDeck();
            gb.PlayerTwo.Cards = PlayerTwoDeck();
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
            var oceanCreator = new OceanCreator();
            var mockBlueCard = new MockColourCard<Blue>();
            var waterSpriteCreator = new WaterSpriteCreator();

            var cardList = new List<Card>();

            // adding concrete cards to the deck 
            for (int i = 0; i < 4; i++)
            {
                cardList.Add(oceanCreator.Create());
            }

            cardList.Add(waterSpriteCreator.Create());
            cardList.Add(mockBlueCard.Create());

            // filling up the rest of the deck with null objects with a white colour (colour doesn't matter here)
            var nullCardCreator = new NullCardCreator<White>();
            while (cardList.Count < 30)
            {
                cardList.Add(nullCardCreator.Create());
            }

            return new Cards(cardList);
        }

        public static Cards PlayerTwoDeck()
        {
            var volcanoCreator = new VolcanoCreator();
            var skyCreator = new SkyCreator();
            var desertCreator = new DesertCreator();

            var cardList = new List<Card>();

            for (int i = 0; i < 3; i++)
            {
                cardList.Add(desertCreator.Create());
            }

            cardList.Add(volcanoCreator.Create());
            cardList.Add(skyCreator.Create());

            // filling up the rest of the deck with null objects with a white colour (colour doesn't matter here)
            var nullCardCreator = new NullCardCreator<White>();
            while (cardList.Count < 30)
            {
                cardList.Add(nullCardCreator.Create());
            }

            return new Cards(cardList);
        }


        // after setting the cards for each player set the starting state
        private static void SetupStartingState()
        {
            var gb = GameBoard.Instance;

            // setting up player one
            var playerOneCards = gb.PlayerOne.Cards;

            Dictionary<string, List<string>> playerOneCardsLocation = new Dictionary<string, List<string>>();
            playerOneCardsLocation.Add("OnBoard", new List<string>() { "ocean", "ocean", "ocean" });
            playerOneCardsLocation.Add("InHand", new List<string>() { "ocean", "water-sprite" });
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
            var onInterrupt = new Effect((_) =>
            {
                var continueInterrupt = false;
                var onYes = () => { continueInterrupt = true; };
                var onNo = () => { };
                var interruptMenu = new YesNoMenu(onYes, onNo);
                interruptMenu.Prompt("Do you wish to interrupt?");
                if (!continueInterrupt) return;

                // for simplicity's sake the player is not able to attack with creatures here
                GameBoard.Instance.CurrentPlayer.ChooseCardInHandToPlay();
            }, Event.INTERRUPT);
            eventHandler.RegisterEffect(onInterrupt);
        }
    }
}
