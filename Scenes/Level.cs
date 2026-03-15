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
    
    public UnitGroupManager PlayerGroup;
    public UnitGroupManager EnemyGroup;
    private GridManager _gridManager;
    private SelectionCursor _selectionCursor;
    private BattleStateMachine _battleStateMachine;
    private Unit _selectedUnit;

    public override void _Ready()
    {
        _gridManager = GetNode<GridManager>("%GridManager");
        PlayerGroup = GetNode<UnitGroupManager>("%PlayerGroup");
        EnemyGroup = GetNode<UnitGroupManager>("%EnemyGroup");

        _battleStateMachine = new BattleStateMachine(this);
        _battleStateMachine.ChangeState(new PlayerTurnState(this));

        PlayerGroup.AllUnitsMoved += OnAllPlayerUnitsMoved;
        EnemyGroup.AllUnitsMoved += OnAllEnemyUnitsMoved;

        GameEvents.Instance.Connect(GameEvents.SignalName.UnitSelected, Callable.From<Unit>(OnUnitSelected));
        GameEvents.Instance.Connect(GameEvents.SignalName.UnitMoved, Callable.From<Unit>(OnUnitMoved));
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    private void OnAllEnemyUnitsMoved()
    {
        GD.Print("All enemy units have moved.");

        _battleStateMachine.ChangeState(new PlayerTurnState(this));
    }

    private void OnAllPlayerUnitsMoved()
    {
        GD.Print("All player units have moved.");

        _battleStateMachine.ChangeState(new EnemyTurnState(this));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _battleStateMachine.CurrentState?.UnhandledInput(@event);
    }

    public override void _PhysicsProcess(double delta)
    {    
        _battleStateMachine.CurrentState.PhysicsProcess(delta);

        if (_selectionCursor != null)
        {
            _selectionCursor.GlobalPosition = _gridManager.MapToLocal(_gridManager.LocalToMap(GetGlobalMousePosition()));
        }
    }   

    private void OnSelectionCursorClick()
    {
        _selectedUnit.Move();
    }

    private void OnUnitMoved(Unit unit)
    {
        RemoveSelectionCursor();
    }

    private void OnUnitSelected(Unit unit)
    {
        _selectedUnit = unit;

        if (!_selectedUnit.HasMoved())
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
        _selectionCursor.SelectionCursorClicked -= OnSelectionCursorClick;
        RemoveChild(_selectionCursor);
    }

}
