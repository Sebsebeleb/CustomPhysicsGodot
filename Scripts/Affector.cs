using System.Collections.Generic;
using System.Linq;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics;

[Tool]
public partial class Affector : Node2D
{
    [Export]
    public float radius
    {
        get => _radius;
        set
        {
            _radius = value; 
            QueueRedraw();
        }
    }

    [Export] public float intensity;
    [Export] public float duration;
    private float _radius;

    private int entityReference;


    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            SetProcess(true);
            return;
        }
            
        this.Setup();
    }

    private void Setup()
    {
        this.entityReference = PhysicsEngine.Spawn(new FakeRBData()
        {
            bodyType = BODY_TYPE.Ephemeral,
            widthOrRadius = this.radius,
            position = this.GlobalPosition,
            components = new List<IComponent>()
            {
                new PushOrPullAffector()
                {
                    intensity = this.intensity
                },
                new VisualComponent()
                {
                    color = new Color(0.7f, 0.3f, 0.8f, 0.5f) * 1.3f,
                },
                new LifetimeComponent()
                {
                    lifetime = this.duration,
                },
            },
        });
    }
    
    public override void _Process(double delta)
    {
        /*
        if (Engine.IsEditorHint())
        {
            return;
        }

        var entity = GetMe();
        if (entity == null)
        {
            QueueFree();
        }
        this.QueueRedraw();
        */
    }

    /*
    private AffectorData GetMe()
    {
        var result = PhysicsEngine.GetRealWorld().Affectors.TryGetValue(this.entityReference, out var data);
        return result ? data : null;
    }
    */

    public override void _Draw()
    {
        /*
        //this.DrawCircle(new Vector2(0,0), this.radius, new Color(0.5f, 0.7f, 1f, 0.6f));

        var data = GetMe();

        this.DrawCircle(new Vector2(0,0), this.radius, new Color(0.12f, 0.03f, 0.16f, 0.2f));
        var remaining = data.remainingDuration  *2;
        var rounded = Mathf.Floor(remaining);
        var fraction = remaining - rounded;
        var frac = (1 - fraction) * 0.95f;
        //var size = this.radius * Mathf.PingPong(frac, 0.5f)*2;
        var size = frac * radius;
        var alpha = 0.5f * fraction;
        this.DrawCircle(new Vector2(0,0), size, new Color(0.54f, 0.2f, 0.6f, alpha));
        base._Draw();
        */
    }

}