using Godot;

namespace GridCombat.Components;

public partial class MovementComponent : Node2D
{
    public bool Moved = false;

    public void Move(Vector2 position)
    {
        GetParent<Node2D>().GlobalPosition = position + new Vector2(-8, -8);
        Moved = true;
    }
}
