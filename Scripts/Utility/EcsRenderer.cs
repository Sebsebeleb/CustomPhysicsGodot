using System.Collections.Generic;
using System.Linq;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Utility;

public partial class EcsRenderer : Node2D
{
    private List<World> queuedData = new List<World>();
    public void SetData(World world)
    {
        this.queuedData.Add(world);
        this.QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (var world in queuedData)
        {
            var query = world.GetComponentsByType<VisualComponent>();
            foreach (var result in query)
            {
                var vComp = result.comp;
                var id = result.entityId;
                var rb = world.FakeRbs.FirstOrDefault(r => r.index == id);
                
                DrawCircle(rb.position, rb.widthOrRadius, vComp.color);
            }
        }
        
        this.queuedData.Clear();
    }
}