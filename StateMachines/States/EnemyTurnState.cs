using Godot;
using GridCombat.Global;
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

        GameEvents.EmitChangeInputBlockStatus(true);

        if (_level.EnemyGroup.Units.Count == 0)
        {
            _level.StateMachine.ChangeState(new WinState(_level));
        }

        foreach (var unit in _level.EnemyGroup.Units)
        {
            await unit.AwaitTime(2);
            unit.AttackOrMove();
        }
    }

    public void Exit()
    {
        foreach (var unit in _level.EnemyGroup.Units)
        {
           unit.Moved = false;
           unit.Attacked = false;
        }

        GameEvents.EmitChangeInputBlockStatus(false);
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