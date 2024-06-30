using System.Linq;
using Godot;

namespace CustomPhysics;

public partial class Character : Node2D
{
    [Export] public float size;

    private int BodyIndex;
    public int GetBodyIndex() => BodyIndex;
    
    public override void _Ready()
    {
        Setup();
    }
    private void Setup()
    {
        BodyIndex = PhysicsEngine.Spawn(new FakeRBData()
        {
            position = this.GlobalPosition,
            bodyType = BODY_TYPE.Body,
            direction = Vector2.Zero,
            speed = 0,
            shapeType = 0,
            widthOrRadius = size,
            height = size
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
}