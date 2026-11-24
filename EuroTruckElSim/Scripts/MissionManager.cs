using Godot;

public partial class MissionManager : Node2D
{
    private readonly MissionProgress _missions = new();
    private readonly Marker2D[] _stops = new Marker2D[3];
    private readonly string[] _names = { "PICKUP", "DELIVERY", "CHARGER" };
    private readonly Color[] _colors = { Colors.Goldenrod, Colors.CornflowerBlue, Colors.MediumSeaGreen };
    private Truck _truck;
    private CollisionShape2D _truckShape;
    private Label _status;
    private ProgressBar _battery;
    private Polygon2D _cargo;
    private bool _interactRequested;

    public override void _Ready()
    {
        _truck = GetNode<Truck>("Truck");
        _truckShape = _truck.GetNode<CollisionShape2D>("CollisionShape2D");
        _stops[0] = GetNode<Marker2D>("Pickup");
        _stops[1] = GetNode<Marker2D>("Delivery");
        _stops[2] = GetNode<Marker2D>("Charger");
        for (int i = 0; i < _stops.Length; i++)
        {
            _stops[i].AddChild(new Label
            {
                Text = _names[i], Position = new Vector2(-100, -100),
                Size = new Vector2(200, 30), HorizontalAlignment = HorizontalAlignment.Center
            });
        }
        _cargo = new Polygon2D
        {
            Polygon = new[] { new Vector2(20, -30), new Vector2(60, -30), new Vector2(60, 0), new Vector2(20, 0) },
            Color = Colors.Goldenrod, Visible = false
        };
        _truck.AddChild(_cargo);
        var hud = new CanvasLayer();
        AddChild(hud);
        var panel = new PanelContainer { Position = new Vector2(16, 16) };
        hud.AddChild(panel);
        var rows = new VBoxContainer();
        panel.AddChild(rows);
        rows.AddChild(new Label { Text = "THREE SIMPLE MISSIONS | A/D or arrows: drive | E: interact" });
        _status = new Label();
        rows.AddChild(_status);
        _battery = new ProgressBar { CustomMinimumSize = new Vector2(400, 24), MaxValue = 100 };
        rows.AddChild(_battery);
        var restart = new Button { Text = "Restart missions", FocusMode = Control.FocusModeEnum.None };
        restart.Pressed += () => GetTree().ReloadCurrentScene();
        rows.AddChild(restart);
        UpdateHud();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.PhysicalKeycode == Key.E)
        {
            _interactRequested = true;
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Input also prevents an interaction from racing a movement tick.
        bool parked = _truck.Velocity.LengthSquared() < 1f
            && Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
        if (_interactRequested)
        {
            for (int i = 0; i < _stops.Length; i++)
                if (AtStop(i)) _missions.Interact((MissionProgress.Stage)i, parked);
            _interactRequested = false;
        }
        _missions.TickCharge(delta, parked && AtStop(2));
        _cargo.Visible = _missions.HasCargo;
        UpdateHud();
        QueueRedraw();
    }

    // The whole existing 200x80 truck fits in a 320x120 bay within these bounds.
    private bool AtStop(int index)
    {
        Vector2 offset = _truckShape.GlobalPosition - _stops[index].GlobalPosition;
        return Mathf.Abs(offset.X) <= 60 && Mathf.Abs(offset.Y) <= 20;
    }

    private void UpdateHud()
    {
        string objective = _missions.Current switch
        {
            MissionProgress.Stage.Pickup => "1/3 Pickup: park in the yellow bay and press E.",
            MissionProgress.Stage.Delivery => "2/3 Delivery: park in the blue bay and press E.",
            MissionProgress.Stage.Charge => "3/3 Charge: stay parked in the green bay until 80%.",
            _ => "3/3 complete! Cargo delivered and battery charged."
        };
        string direction = "";
        if (_missions.Current != MissionProgress.Stage.Complete)
        {
            int target = (int)_missions.Current;
            float distance = _stops[target].GlobalPosition.X - _truckShape.GlobalPosition.X;
            direction = AtStop(target) ? " | At destination" : distance > 0 ? " | Drive RIGHT >>" : " | << Drive LEFT";
        }
        _status.Text = objective + direction + "\nCargo: " + (_missions.HasCargo ? "1 crate" : "empty")
            + $" | Battery: {_missions.BatteryPercent:0}%";
        _battery.Value = _missions.BatteryPercent;
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-600, 90, 3200, 40), new Color(0.2f, 0.23f, 0.28f));
        for (int i = 0; i < _stops.Length; i++)
        {
            if (_stops[i] == null) continue;
            var bay = new Rect2(_stops[i].Position - new Vector2(160, 60), new Vector2(320, 120));
            DrawRect(bay, new Color(_colors[i], 0.18f));
            DrawRect(bay, _colors[i], false, (int)_missions.Current == i ? 5f : 2f);
        }
    }
}
