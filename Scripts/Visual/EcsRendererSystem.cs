using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using CustomPhysics.Utility;
using Godot;

namespace CustomPhysics.Visual;

[SystemType]
public class EcsRendererSystem : BaseSystem
{
    public static Node2D _renderingNode;
    public override void Initialize()
    {
    }

    public override void OnRealWorldStep(World world)
    {
        EcsRendererNode.SetData(world);
    }
}