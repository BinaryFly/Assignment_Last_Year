/*
jeroen visser 0952491
*/
// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Support;
internal class Program
{

    public static void Main(string[] args)
    {
        var referencedAssemblies = Assembly.GetExecutingAssembly().GetTypes().Select((t) => t.Name);
        foreach (string assembly in referencedAssemblies) {
            Console.WriteLine(assembly);
        }
        if (Constants.DEBUG)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("In DEBUG/DEMO mode, set DEBUG const under Support.Constants to false to switch from the hardcoded script to play the actual game");
            Console.ResetColor();
        }
        Console.WriteLine("Preparing Demo...");
        Setup.SetupDemoSituation();
        Setup.SetupDefaultEffects();

        Console.WriteLine("Starting Game!");
        // creating the game loop
        var gameEnded = false;
        EventReactor eventHandler = GameBoard.Instance;

        eventHandler.RegisterEffect(new DefaultEffect(() => { gameEnded = true; }, Event.PLAYERDIED));

        while (!gameEnded)
        {
            GameBoard.Instance.HandlePhase();
        }
    }
}
