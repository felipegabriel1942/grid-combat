using Godot;

namespace GridCombat.StateMachines.States;

public interface IBattleState
{
    
    void Enter();
    void Exit();
    void Update(double delta);
    void PhysicsProcess(double delta);
    void UnhandledInput(InputEvent @event);

}
