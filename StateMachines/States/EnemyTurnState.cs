using Godot;
using GridCombat.Scenes;

namespace GridCombat.StateMachines.States;

public class EnemyTurnState : IBattleState
{

    Level _level;

    public EnemyTurnState(Level level)
    {
        _level = level;
    }

    public async void Enter()
    {
        GD.Print("Begin enemy turn!");

        foreach (var unit in _level.EnemyGroup.Units)
        {
           unit.SetMovedToFalse();
        }

        foreach (var unit in _level.EnemyGroup.Units)
        {
            await unit.AwaitTime(2);
            unit.Move();
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