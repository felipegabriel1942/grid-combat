using System.Threading.Tasks;
using Godot;

namespace GridCombat.Components;

public partial class MovementComponent : Node2D
{
    public async Task Move(Vector2 position)
    {
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(GetParent<Node2D>(), "global_position", position + new Vector2(-8, -8), 0.4);

        await ToSignal(tween, "finished");

        tween.Dispose();
        
    }
}
