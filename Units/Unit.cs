using System.Threading.Tasks;
using Godot;
using GridCombat.Components;
using GridCombat.Managers;

namespace GridCombat.Units;

public abstract partial class Unit : Node2D
{

    [Signal]
    public delegate void UnitHasMovedEventHandler();

    [Signal]
    public delegate void UnitHasAttackedEventHandler();

    [Signal]
    public delegate void UnitHasDiedEventHandler(Unit unit);

    [Export]
    public GridManager GridManager { private set; get; }

    [Export]
    public HighlightManager HighlightManager { private set; get; }

    [Export]
    public int AttackRange = 1;

    [Export]
    public int MovementRange = 1;

    [Export]
    public int Health = 3;

    public bool Moved = false;
    public bool Attacked = false;
    public int CurrentHealth;

    protected MovementComponent _movementComponent;
    protected SelectionComponent _selectionComponent;
    protected AnimationComponent _animationComponent;
    protected AudioStreamPlayer _weaponSound;
    protected AudioStreamPlayer _deathSound;
    protected Node2D _spritesContainer;
    protected Node2D _healthContainer;

    protected bool _isSelected;

    public override void _Ready()
    {
        _movementComponent = GetNode<MovementComponent>("MovementComponent");
        _selectionComponent = GetNode<SelectionComponent>("SelectionComponent");
        _animationComponent = GetNode<AnimationComponent>("AnimationComponent");
        _weaponSound = GetNode<AudioStreamPlayer>("WeaponSound");
        _deathSound = GetNode<AudioStreamPlayer>("DeathSound");
        _spritesContainer = GetNode<Node2D>("SpritesContainer");
        _healthContainer = GetNode<Node2D>("HealthContainer");

        _selectionComponent.Selected += OnSelection;

        GridManager.SetCellAsOccupied(GlobalPosition);

        CurrentHealth = Health;

        UpdateHealthContainer();
    }

    public abstract void OnSelection();

    public abstract void AttackOrMove();

    public abstract void Attack(Unit target);

    public abstract void Move();

    public async Task AwaitTime(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), Timer.SignalName.Timeout);
    }

    public abstract void TakeDamage(int damage);

    public async void Die()
    {
        GridManager.SetCellAsFree(GlobalPosition);
        EmitSignal(SignalName.UnitHasDied, this);
        _deathSound.Play();
        _animationComponent.PlayDeath();
        await ToSignal(_deathSound, AudioStreamPlayer.SignalName.Finished);
        QueueFree();
    }

    public abstract void PlayWeaponSound();

    public void FlipUnit(string direction)
    {
        if (direction == "Left")
        {
            _spritesContainer.Scale = new Vector2(-1, 1);
            _spritesContainer.Position = new Vector2(16, 0);
        }
        else
        {
            _spritesContainer.Scale = new Vector2(1, 1);
            _spritesContainer.Position = new Vector2(0, 0);
        }

    }

    public void UpdateHealthContainer()
    {

        foreach (var child in _healthContainer.GetChildren())
        {
            child.QueueFree();
        }

        for (int i = 0; i < Health; i++)
        {
            var sprite = new Sprite2D();

            var texture = i < CurrentHealth ? GetHealthFilledTexture() : GD.Load<Texture2D>("res://Assets/UI/HealthEmpty.png");

            sprite.Texture = texture;

            sprite.GlobalPosition = new Vector2(5 + (i * 5), sprite.GlobalPosition.Y + 2);
            sprite.ZIndex = 5;

            _healthContainer.AddChild(sprite);
        }
    }


    public abstract Texture2D GetHealthFilledTexture();
}
