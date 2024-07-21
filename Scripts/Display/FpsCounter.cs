using Godot;

namespace CustomPhysics.Display;

public partial class FpsCounter : Label
{
    public override void _Process(double delta)
    {
        this.Text = Engine.GetFramesPerSecond().ToString();
    }
}