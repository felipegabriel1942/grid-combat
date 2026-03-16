using Godot;
using GridCombat.Scenes;

namespace GridCombat.StateMachines.States;

public class LoseState : IBattleState
{

    Level _level;

    public LoseState(Level level)
    {
        _level = level;
    }

    public void Enter()
    {
        GD.Print("You lose the battle!");
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