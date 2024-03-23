/*
jeroen visser 0952491
*/
namespace Support;

class Logger 
{
    public static void Situation() {
        var gb = GameBoard.Instance;
        MakeTitle("Current Situation");
        VerticalLine($"Turn {gb.CurrentPlayer.Turn} of {gb.CurrentPlayer.GetName()}");
        VerticalLine($"Player {gb.PlayerOne.GetName()}: Health: {gb.PlayerOne.Health}");
        VerticalLine($"Player {gb.PlayerTwo.GetName()}: Health: {gb.PlayerTwo.Health}");
        VerticalLine("");
        MakeTitle($"Player {gb.PlayerOne.GetName()} cards overview");
        System.Console.WriteLine(gb.PlayerOne.Deck);
        VerticalLine("");
        MakeTitle($"Player {gb.PlayerTwo.GetName()} cards overview");
        System.Console.WriteLine(gb.PlayerTwo.Deck);
        System.Console.WriteLine(new string('-', Constants.MENU_WIDTH));
    }

    public static void MakeTitle(string title)
    {
        var totalLength = Constants.MENU_WIDTH;
        var prefixLength = 6;
        var titleLength = title.Length;
        var suffixLength = totalLength - prefixLength - titleLength - 1;
        Console.WriteLine($"----- {title} {new string('-', suffixLength)}");
    }

    public static void VerticalLine(string line)
    {
        Console.WriteLine(String.Format("| {0, -" + (Constants.MENU_WIDTH - 4) + "} |", line));
    }

    public static void HorizontalLine()
    {
        Console.WriteLine(new string('-', Constants.MENU_WIDTH));
    }
}
