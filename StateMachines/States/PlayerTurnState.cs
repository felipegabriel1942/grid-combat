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
        GD.Print("Begin player turn!");

        if (_level.PlayerGroup.Units.Count == 0)
        {
            _level.StateMachine.ChangeState(new LoseState(_level));
        }

        foreach (var unit in _level.PlayerGroup.Units)
        {
           unit.Moved = false;
           unit.Attacked = false;
        }
    }

    public void Exit()
    {

    }

    public void PhysicsProcess(double delta)
    {

    }

    public void UnhandledInput(InputEvent @event)
    {
    }

    public void Update(double delta)
    {
        
    }
}