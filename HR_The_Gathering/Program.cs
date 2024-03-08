/*
jeroen visser 0952491
*/
// See https://aka.ms/new-console-template for more information
using Support;
internal class Program
{

    public static void Main(string[] args)
    {
        if (Constants.DEBUG)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("In DEBUG/DEMO mode, set DEBUG const under Support.Constants to false to switch from the hardcoded script to play the actual game");
            Console.ResetColor();
        }
        Console.WriteLine("Preparing Demo...");
        // SetUpGameBoard();  // useful for not demoing
        Setup.SetupDemoSituation();
        Setup.SetupDefaultEffects();

        Console.WriteLine("Starting Game!");
        // creating the game loop
        var gameEnded = false;
        EventReactor eventHandler = GameBoard.Instance;

        eventHandler.RegisterEffect(new Effect((_) => { gameEnded = true; }, Event.PLAYERDIED));
        while (!gameEnded)
        {
            GameBoard.Instance.HandlePhase();
        }
    }

    public static void SetUpGameBoard()
    {
        GameBoard gameboard = GameBoard.Instance;

        Player playerOne = new Player("Arnold");
        Player playerTwo = new Player("Bryce");

        gameboard.PlayerOne = playerOne;
        gameboard.PlayerTwo = playerTwo;
    }
}
