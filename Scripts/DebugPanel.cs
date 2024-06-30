using Godot;

namespace CustomPhysics;

public partial class DebugPanel : Node
{
    public void _on_body_prediction_toggled(bool newValue)
    {
        DebugSettings.ShowBodyPredictions = newValue;
    }
}