using Godot;

namespace CustomPhysics.Scripts;

public partial class PlayerInput : Node
{
    [Export] private PackedScene gravityWell;

    private bool changingTime = false;
    public float targetFlowSpeed = 1f;

    public override void _Input(InputEvent @event)
    {
        bool changedWorld = false;
        if (@event is InputEventMouseButton mb && @event.IsPressed())
        {
            if (mb.ButtonIndex == MouseButton.Right)
            {
                var mbpos = GetViewport().GetMousePosition();
                var well = gravityWell.Instantiate<Node2D>();
                well.GlobalPosition = mbpos;
                GetTree().Root.AddChild(well);
                changedWorld = true;
            }
            if (mb.ButtonIndex == MouseButton.WheelUp || mb.ButtonIndex == MouseButton.WheelDown)
            {
                float delta = 0;
                if (mb.ButtonIndex == MouseButton.WheelUp)
                {
                    delta += 0.015f;
                }
                else if (mb.ButtonIndex == MouseButton.WheelDown)
                {
                    delta -= 0.015f;
                }

                float min = 0;
                float max = 1f;
                
                var current = TimeControl.TimeScale;
                current += delta;

                current = Mathf.Clamp(current, min, max);
                TimeControl.TimeScale = current;
            }
        }

        if (@event.IsActionPressed("StopTime"))
        {
            changingTime = true;
            targetFlowSpeed = 0f;
        }

        if (@event.IsActionPressed("SetToRegularFlow"))
        {
            changingTime = true;
            targetFlowSpeed = 1f;
        }

        if (changedWorld)
        {
            PhysicsEngine.PredictFuturePlease();
        }
    }

    public override void _Process(double delta)
    {
        if (changingTime)
        {
            var current = TimeControl.TimeScale;
            var sign = current < targetFlowSpeed ? 1 : -1;
            var bonusMultiplier = sign < 0 ? 5 : 1;
            current += (float)delta * 2 * Mathf.Max(current, 0.1f) * sign * bonusMultiplier;

            current = Mathf.Clamp(current, 0, 1);
            TimeControl.TimeScale = current;

            if (sign > 0 && current > targetFlowSpeed)
            {
                current = targetFlowSpeed;
            }
            else if (sign < 0 && current < targetFlowSpeed)
            {
                current = targetFlowSpeed;
            }

            if (Mathf.IsEqualApprox(targetFlowSpeed, current))
            {
                changingTime = false;
            }
        }
    }
}