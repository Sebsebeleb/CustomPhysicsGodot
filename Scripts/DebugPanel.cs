using Godot;

namespace CustomPhysics;

public partial class DebugPanel : Node
{

    public void _on_body_prediction_toggled(bool newValue)
    {
        DebugSettings.ShowBodyPredictions = newValue;
    }
    
    public void _on_body_alternate_color_toggled(bool newValue)
    {
        DebugSettings.AlternateBodyPredictionColors = newValue;
    }

    public void _on_prediction_steps_value_changed(float newValue)
    {
        int v = (int)newValue;
        
        PhysicsEngine.SetPredictionSteps(v);
    }

    public void _on_prediction_time_value_changed(float newValue){
        
        PhysicsEngine.SetPredictionTime(newValue);
        
    }
}