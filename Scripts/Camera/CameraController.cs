using Godot;
using System;

public partial class CameraController : Camera2D
{

    [Export] private float speedSensitivity;

    [Export] private Curve zoomCurve;
    
    private float currentZoom;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            bool ctrlPressed = Input.IsPhysicalKeyPressed(Key.Ctrl);
            if (ctrlPressed && @event.IsPressed() &&
                (mb.ButtonIndex == MouseButton.WheelUp || mb.ButtonIndex == MouseButton.WheelDown))
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
                
                GD.Print("Delta: " + delta);

                this.currentZoom += delta * speedSensitivity;
                this.currentZoom = Mathf.Clamp(this.currentZoom, 0, 1);
                this.Zoom = Vector2.One * this.zoomCurve.Sample(this.currentZoom);

            }
        }
    }
}