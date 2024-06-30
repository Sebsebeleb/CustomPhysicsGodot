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
        Engine.GetMainLoop();
        base.Initialize();
    }

    public override void Update(World world)
    {
        base.Update(world);

        EcsRendererNode.SetData(world);
    }
}