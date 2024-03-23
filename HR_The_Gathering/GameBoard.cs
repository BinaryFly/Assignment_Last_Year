/*
jeroen visser 0952491
*/
// this will become the mediator object for the rest of the game, deciding what can and what can't be done
using Support;

class GameBoard : EventReactor
{
    private static GameBoard? instance;
    private Player playerOne;
    private Player playerTwo;
    private Player currentPlayer;
    private Player opponent;
    private Phase currentPhase;

    private GameBoard()
    {
        // we mainly set the players here already to avoid intellisense warnings
        this.playerOne = new Player("dummy");
        this.playerTwo = new Player("dummy");
        this.currentPlayer = this.playerOne;
        this.opponent = this.playerTwo;

        this.currentPhase = new Preparation();
    }

    public static GameBoard Instance
    {
        get
        {
            if (instance == null)
            {
                Console.WriteLine("Creating GameBoard Instance");
                instance = new GameBoard();
            }
            return instance;
        }
    }

    public Player PlayerOne { get => playerOne; }
    public Player PlayerTwo { get => playerTwo; }

    public Player CurrentPlayer { get => currentPlayer; }
    public Player Opponent { get => opponent; }

    public void HandlePhase()
    {
        this.currentPhase.HandlePhase();
    }

    public void UpdatePhase(Phase newPhase)
    {
        this.currentPhase = newPhase;
    }

    /// <summary>
    /// Sets the first argument as PlayerOne and the CurrentPlayer.
    /// Sets the second argument as PlayerTwo and the Opponent.
    /// </summary>
    public void SetPlayers(Player currentPlayer, Player opponent) 
    {
        this.playerOne = currentPlayer;
        this.currentPlayer = currentPlayer;
        this.playerTwo = opponent;
        this.opponent = opponent;
    }

    public void SwitchPlayers()
    {
        if (this.currentPlayer == this.PlayerOne)
        {
            this.opponent = this.PlayerOne;
            this.currentPlayer = this.PlayerTwo;
            return;
        }
        this.opponent = this.PlayerTwo;
        this.currentPlayer = this.PlayerOne;
    }


    public void GameOver()
    {
        // FUTURE: if the player can be revived or doesn't necessarily stay dead after dying do not unregister all the effects
        this.UnRegisterAll();
        UpdatePhase(new Ending());
        AnnounceGameWinner();
    }

    private void AnnounceGameWinner()
    {
        if (PlayerOne.IsDead)
        {
            Console.WriteLine($"Player {PlayerOne.GetName()} died, Player {PlayerTwo.GetName()} won!");
        }
        else if (PlayerTwo.IsDead)
        {
            Console.WriteLine($"Player {PlayerTwo.GetName()} died, Player {PlayerOne.GetName()} won!");
        }
        else {
            // this is handy if we forcefully quit the game after the Debug situation
            Console.WriteLine($"Game ended");
        }
    }

    public Menu MainPhaseMenu()
    {
        var playCard = new MenuItem("Play Card", () => { CurrentPlayer.ChooseCardInHandToPlay(); });
        var creatureAttack = new MenuItem("Attack With Creature", () =>
        {
            var targetsToAttack = GetTargets();
            var creaturesAbleToAttack = CurrentPlayer.GetDefendingCreatures();
            var creatureMenu = new CardMenu<Creature>(creaturesAbleToAttack, (creature) =>
            {
                var targetMenu = new TargetMenu(targetsToAttack, (target) => { creature.PerformAttack(target); });
                targetMenu.Prompt($"Which target do you wish to attack with {creature.Id}?:");
            });

            creatureMenu.Prompt("With which creature do you wish to attack?");
        });
        // ending the turn is defined outside of this function
        return new Menu(new List<MenuItem>() { playCard, creatureAttack });
    }

    // handy for the opponent to get all the targets that he is able to attack
    public List<Target> GetTargets()
    {
        var defendingCreatures = Opponent.GetDefendingCreatures();
        if (defendingCreatures.Count > 0)
        {
            // return all creatures as targets whenever there are defending creatures
            // the player should also be able to attack attacking creatures if there are defending ones
            return Opponent.Deck.OnBoard.Creatures.ToList<Target>();
        }
        return new List<Target>() { Opponent };
    }
}

