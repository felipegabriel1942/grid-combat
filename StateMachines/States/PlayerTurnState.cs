using Godot;
using GridCombat.Global;
using GridCombat.Scenes;
using GridCombat.Units.Player;

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
    }

    public void Exit()
    {
        foreach (var unit in _level.PlayerGroup.Units)
        {
            if (unit is Player player)
            {
                player.Moved = false;
                player.Attacked = false;
                player.ChangeSpritesColor();
            }
        }
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