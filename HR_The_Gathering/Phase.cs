/*
jeroen visser 0952491
*/
using Support;

interface Phase
{
    void HandlePhase();
    void NextPhase();
}

class Preparation : Phase
{
    public void HandlePhase()
    {
        GameBoard.Instance.HandleEvents(new List<Event>() { Event.LOG_NEW_TURN, Event.ENTER_PREPARATION_PHASE });
        var player = GameBoard.Instance.CurrentPlayer;
        player.IncrementTurn();
        player.ResetLands();
        player.ResetCreatures();
        NextPhase();
        GameBoard.Instance.HandleEvent(Event.EXIT_PREPARATION_PHASE);
    }

    public void NextPhase()
    {
        GameBoard.Instance.UpdatePhase(new Drawing());
    }
}

class Drawing : Phase
{
    public void HandlePhase()
    {

        GameBoard.Instance.HandleEvent(Event.ENTER_DRAW_PHASE);
        var player = GameBoard.Instance.CurrentPlayer;
        player.DrawCard();
        NextPhase();
        GameBoard.Instance.HandleEvent(Event.EXIT_DRAW_PHASE);
    }

    public void NextPhase()
    {
        GameBoard.Instance.UpdatePhase(new Main());
    }
}

class Main : Phase
{
    public void HandlePhase()
    {
        // extra true call here to ensure that mainPhase is true when entering this effect
        var actionMenu = GameBoard.Instance.MainPhaseMenu();
        actionMenu.AddOption(new MenuItem("End Turn", () => { NextPhase(); }));
        actionMenu.Prompt("What do you wish to do?:");
    }

    public void NextPhase()
    {
        GameBoard.Instance.UpdatePhase(new Ending());
    }
}


class Ending : Phase
{
    public void HandlePhase()
    {
        GameBoard.Instance.HandleEvent(Event.ENTER_ENDING_PHASE);
        var player = GameBoard.Instance.CurrentPlayer;
        player.DiscardExcessCards();
        GameBoard.Instance.SwitchPlayers();
        NextPhase();
        GameBoard.Instance.HandleEvent(Event.EXIT_ENDING_PHASE);
    }

    public void NextPhase()
    {
        GameBoard.Instance.UpdatePhase(new Preparation());
    }
}
