using Godot;
using GridCombat.Scenes;
using GridCombat.StateMachines.States;

namespace GridCombat.StateMachines;

public partial class BattleStateMachine : Node
{
    
    private Level _level;

    public IBattleState CurrentState { get; private set; }

    public BattleStateMachine(Level level)
    {
        _level = level;
    }

    public void ChangeState(IBattleState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

}
