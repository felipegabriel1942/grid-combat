using System.Threading.Tasks;
using Godot;

namespace GridCombat.Components;

public partial class AnimationComponent : Node2D
{
    [Export]
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        var animation = _animationPlayer.GetAnimation("Idle");

        float randomTime = (float) GD.RandRange(0, animation.Length);

        _animationPlayer.Play("Idle");
        _animationPlayer.Seek(randomTime, true);
    }

    public async void PlayAttack()
    {
        _animationPlayer.Play("Attack");

        await ToSignal(_animationPlayer, "animation_finished");

        _animationPlayer.Play("Idle");
    }

    public async Task PlayHurt()
    {
        _animationPlayer.Play("Hurt");

        await ToSignal(_animationPlayer, "animation_finished");

        _animationPlayer.Play("Idle");
    }

    public async void PlayDeath()
    {
        await ToSignal(GetTree().CreateTimer(0.3f), Timer.SignalName.Timeout);
        _animationPlayer.Play("Death");
    }
}
