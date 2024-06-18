using System;
using System.Linq;
using Godot;

namespace CustomPhysics.Scripts;

public enum BODY_TYPE
{
    /// <summary>
    /// Will never be affected by physics
    /// </summary>
    Static,
    /// <summary>
    /// Affected by impacts and forces, like from affectors
    /// </summary>
    Projectile,
    /// <summary>
    /// Not impacted by certain forces like affectors, and their direction/speed is (for now at least) not affected by gravity. They control their own movement 100%
    /// </summary>
    Body,
}

public class FakeRBData
{
    public int index;
    public BODY_TYPE bodyType;
    public Vector2 position;
    [Export] public Vector2 direction;
    [Export] public float speed;
    [Export] public int shapeType;
    [Export] public float widthOrRadius;
    [Export] public float height;
    [Export] public int bounces = 0;
}
public partial class FakeRB : Node2D
{
    private int BodyIndex = -1;

    public Vector2 position;
    [Export] public Vector2 direction;
    [Export] public float speed;
    [Export] public int shapeType;
    [Export] public float widthOrRadius;
    [Export] public float height;
    [Export] public int bounces = 0;

    private int frameCounter;

    public int GetBodyIndex() => BodyIndex;
    public override void _Ready()
    {
        Setup();
    }

    public override void _EnterTree()
    {
        
    }

    private void Setup()
    {
        BodyIndex = PhysicsEngine.Spawn(new FakeRBData()
        {
            position = this.GlobalPosition,
            bodyType = BODY_TYPE.Projectile,
            direction = direction.Normalized(),
            speed = speed,
            shapeType = shapeType,
            widthOrRadius = widthOrRadius,
            height = height
        });
    }
    

    public override void _Process(double delta)
    {
        this.QueueRedraw();
        var body = PhysicsEngine.GetRealWorld().FakeRbs.FirstOrDefault(rb => rb.index == BodyIndex);
        if (body == null)
        {
            QueueFree();
            return;
        }


        this.GlobalPosition = body.position;
    }

    private const int stepsToSkip = 1;
    public override void _Draw()
    {
        bool first = true;
        for (var index = 0; index < PhysicsEngine.PredictionWorldData.Length; index++)
        {
            if (index % stepsToSkip != 0)
            {
                continue;
            }
            var world = PhysicsEngine.PredictionWorldData[index];
            var me = world.FakeRbs.FirstOrDefault(b => b.index == this.BodyIndex);
            if (me == null)
            {
                break;
            }
            

            if (first)
            {
                var position = CalculateExtrapolatedPosition();
                DrawArc(position-GlobalPosition, me.widthOrRadius, 0, 360, 24, Colors.Yellow, 3);
                first = false;
            }
            else if (DebugSettings.ShowBodyPredictions)
            {
                DrawArc(me.position-GlobalPosition, me.widthOrRadius, 0, 360, 24, new Color(1, 1, 1, 0.2f), 1.5f);
            }
        }
        base._Draw();
    }
    
    public Vector2 CalculateExtrapolatedPosition()
    {
        if (PhysicsEngine.PredictionWorldData.Length > 1)
        {
            var currentMe = PhysicsEngine.PredictionWorldData[0].FakeRbs.FirstOrDefault(r => r.index == this.BodyIndex);
            var nextMe = PhysicsEngine.PredictionWorldData[1].FakeRbs.FirstOrDefault(r => r.index == this.BodyIndex);

            double factor = PhysicsEngine.GetFrameFraction();
            var extrapolated = currentMe.position.Lerp(nextMe.position, (float)factor);
            return extrapolated;
        }
        else
        {
            GD.PrintErr("Missing world");
        }

        return new Vector2();
    }
}