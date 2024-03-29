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
        Setup.SetupDemoSituation();
        Setup.SetupDefaultEffects();

        Console.WriteLine("Starting Game!");
        Setup.RunDemo();
    }
}
