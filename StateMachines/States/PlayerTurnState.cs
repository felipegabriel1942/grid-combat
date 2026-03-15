using Godot;
using GridCombat.Scenes;

namespace GridCombat.StateMachines.States;

public class PlayerTurnState : IBattleState
{

    Level _level;

    public PlayerTurnState(Level level)
    {
        _level = level;
    }


    public void Enter()
    {
    }

    public void Exit()
    {

    }

    public void PhysicsProcess(double delta)
    {

    }

    public void UnhandledInput(InputEvent @event)
    {
        // if (@event.IsActionPressed("select"))
        // {
        //     if (_level.SelectedUnit == null)
        //     {
        //         _level.SelectUnit(); 
        //     } else
        //     {
        //         // _level.MoveUnit();
        //     }  
        // }
    }

    public void Update(double delta)
    {
        
    }
}