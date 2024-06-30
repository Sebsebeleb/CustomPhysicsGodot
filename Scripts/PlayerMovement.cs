using System.Linq;
using Godot;

namespace CustomPhysics;

public partial class PlayerMovement : Node2D
{
    [Export]
    private Character rb;

    [Export] public float movementSpeed;

    public override void _Process(double delta)
    {
        var horizontal = Input.GetAxis("left", "right");
        var vertical = Input.GetAxis("up", "down");

        var ourIndex = rb.GetBodyIndex();
        var realWorld = PhysicsEngine.GetRealWorld();
        var me = realWorld.FakeRbs.FirstOrDefault(r => r.index == ourIndex);
        var dir = new Vector2(horizontal, vertical);
        if (dir.Length() > 1)
        {
            dir = dir.Normalized();
        }
        me.direction = dir;
        me.speed = dir.Length() * movementSpeed;
        base._Process(delta);
    }
}