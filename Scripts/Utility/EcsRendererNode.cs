using System.Collections.Generic;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Utility;

public partial class EcsRendererNode : Node
{
    private static EcsRendererNode instance;
    private EcsRenderer renderer;

    private List<World> queuedData = new List<World>();

    public override void _EnterTree()
    {
        if (instance != null)
        {
            GD.PrintErr("Uhh something has gone wrong, singleton instance already filled: " + instance);
        }
        instance = this;
        var n = new Node2D();
        renderer = new EcsRenderer();
        n.SetScript(ResourceLoader.Load("res://Scripts/Utility/"+nameof(EcsRenderer)+".cs"));
        this.AddChild(renderer);
    }

    public override void _ExitTree()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void SetData(World world)
    {
        instance.DoSetData(world);
    }

    private void DoSetData(World world)
    {
        this.renderer.SetData(world);
    }
}