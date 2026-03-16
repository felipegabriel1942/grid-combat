using Godot;
using GridCombat.Scenes;

namespace GridCombat.StateMachines.States;

public class WinState : IBattleState
{

    Level _level;

    public WinState(Level level)
    {
        _level = level;
    }

    public void Enter()
    {
        GD.Print("You won the battle!");
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