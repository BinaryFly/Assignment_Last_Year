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

    public Player PlayerOne
    {
        get => playerOne;
        set
        {
            SetPlayerIfDummy(value);
            playerOne = value;
        }
    }

    public Player PlayerTwo
    {
        get => playerTwo;
        set
        {
            SetPlayerIfDummy(value);
            playerTwo = value;
        }
    }

    public Player CurrentPlayer
    {
        get => currentPlayer; set
        {
            currentPlayer = value;
            // automatically set the opponent whenever the current player changes
            if (this.currentPlayer == this.playerOne)
            {
                this.opponent = this.playerTwo;
                return;
            }
            this.opponent = this.playerOne;
        }
    }

    public Player Opponent { get => opponent; }

    public void HandlePhase()
    {
        this.currentPhase.HandlePhase();
    }

    public void UpdatePhase(Phase newPhase)
    {
        this.currentPhase = newPhase;
    }

    // if the current player is still a dummy player, set the newly set player also as the current player
    private void SetPlayerIfDummy(Player playerToSet)
    {
        if (playerToSet.Name == "dummy") return;

        if (this.CurrentPlayer.Name == "dummy")
        {
            this.CurrentPlayer = playerToSet;
        }
        else if (this.Opponent.Name == "dummy")
        {
            this.opponent = playerToSet;
        }
    }

    public void SwitchPlayers()
    {
        if (this.currentPlayer == this.PlayerOne)
        {
            this.CurrentPlayer = this.PlayerTwo;
            return;
        }
        this.CurrentPlayer = this.PlayerOne;
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
        else
        {
            Console.WriteLine($"Player {PlayerTwo.GetName()} died, Player {PlayerOne.GetName()} won!");
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
            return Opponent.Cards.OnBoard.Creatures.ToList<Target>();
        }
        return new List<Target>() { Opponent };
    }

    // logs the relevant situation
    public void LogSituation()
    {
        Console.WriteLine(@$"
===== Current Situation =====                               
Turn {currentPlayer.Turn} of {currentPlayer.GetName()}
Player {PlayerOne.GetName()}: Health: {PlayerOne.Health}
Player {PlayerTwo.GetName()}: Health: {PlayerTwo.Health}

===== Player {PlayerOne.GetName()} cards overview =====
{PlayerOne.Cards.ToString()}
 
===== Player {PlayerTwo.GetName()} cards overview =====
{PlayerTwo.Cards.ToString()}
");
    }
}

