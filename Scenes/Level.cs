using Godot;
using GridCombat.Global;
using GridCombat.Managers;
using GridCombat.StateMachines;
using GridCombat.StateMachines.States;
using GridCombat.UI;
using GridCombat.Units;

namespace GridCombat.Scenes;

public partial class Level : Node2D
{
    [Export]
    private PackedScene SelectionCursorScene;

    public Unit SelectedUnit {private set; get; }

    public BattleStateMachine StateMachine;
    
    public UnitGroupManager PlayerGroup;
    public UnitGroupManager EnemyGroup;

    private GridManager _gridManager;
    private SelectionCursor _selectionCursor;
    private Unit _selectedUnit;
    private bool _inputBlocked = false;

    public override void _Ready()
    {
        _gridManager = GetNode<GridManager>("%GridManager");
        PlayerGroup = GetNode<UnitGroupManager>("%PlayerGroup");
        EnemyGroup = GetNode<UnitGroupManager>("%EnemyGroup");

        StateMachine = new BattleStateMachine(this);
        StateMachine.ChangeState(new PlayerTurnState(this));

        PlayerGroup.AllUnitsMoved += OnAllPlayerUnitsMoved;
        EnemyGroup.AllUnitsMoved += OnAllEnemyUnitsMoved;

        PlayerGroup.AllUnitsDied += OnAllPlayerUnitsDied;
        EnemyGroup.AllUnitsDied += OnAllEnemyUnitsDied;

        GameEvents.Instance.Connect(GameEvents.SignalName.UnitSelected, Callable.From<Unit>(OnUnitSelected));
        GameEvents.Instance.Connect(GameEvents.SignalName.UnitMoved, Callable.From<Unit>(OnUnitMoved));
        GameEvents.Instance.Connect(GameEvents.SignalName.ChangeInputBlockStatus, Callable.From<bool>(OnBlockInputStatusChange));
    }

    private void OnBlockInputStatusChange(bool isBlocked)
    {
        _inputBlocked = isBlocked;

        RemoveSelectionCursor();
    }

    private void OnAllEnemyUnitsDied()
    {
        StateMachine.ChangeState(new WinState(this));
    }

    private void OnAllPlayerUnitsDied()
    {
        StateMachine.ChangeState(new LoseState(this));
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    private async void OnAllEnemyUnitsMoved()
    {
        GD.Print("All enemy units have moved.");

        await ToSignal(GetTree().CreateTimer(5f), Timer.SignalName.Timeout);

        StateMachine.ChangeState(new PlayerTurnState(this));
    }

    private async void OnAllPlayerUnitsMoved()
    {
        GD.Print("All player units have moved.");

        await ToSignal(GetTree().CreateTimer(5f), Timer.SignalName.Timeout);

        StateMachine.ChangeState(new EnemyTurnState(this));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        StateMachine.CurrentState?.UnhandledInput(@event);
    }

    public override void _PhysicsProcess(double delta)
    {    
        StateMachine.CurrentState.PhysicsProcess(delta);

        if (_selectionCursor != null)
        {
            _selectionCursor.GlobalPosition = _gridManager.MapToLocal(_gridManager.LocalToMap(GetGlobalMousePosition()));
        }
    }   

    private void OnSelectionCursorClick()
    {
        if (_inputBlocked)
            return;

        _selectedUnit.AttackOrMove();
    }

    private void OnUnitMoved(Unit unit)
    {
        RemoveSelectionCursor();
    }

    private void OnUnitSelected(Unit unit)
    {
        if (_inputBlocked)
            return;
            
        _selectedUnit = unit;

        if (!_selectedUnit.Moved || !_selectedUnit.Attacked)
        {
            InstantiateSelectionCursor();
        }
    }

    public void InstantiateSelectionCursor()
    {
        var selectionCursor = SelectionCursorScene.Instantiate<SelectionCursor>();
        _selectionCursor = selectionCursor;
        _selectionCursor.SelectionCursorClicked += OnSelectionCursorClick;
        
        AddChild(_selectionCursor);
    }

    private void RemoveSelectionCursor()
    {
        if (_selectionCursor == null)
        {
            return;
        }

        _selectionCursor.SelectionCursorClicked -= OnSelectionCursorClick;
        RemoveChild(_selectionCursor);
        _selectionCursor = null;
    }

}
