using System.Collections.Generic;
using System.Linq;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Character;
using CustomPhysics.Gameplay.Components;
using CustomPhysics.Gameplay.Tags;
using CustomPhysics.Gameplay.Weapons;
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
            height = size,
            components = new List<IComponent>()
            {
                new VisualComponent()
                {
                    type = VisualComponent.VisualType.Sprite,
                    //type = VisualComponent.VisualType.Shape,
                    spriteID = 0,
                    size = new Vector2(32, 32),
                },
                new PlayerTag(),
                new WeaponComponent()
                {
                    StatAttackRate = 2f,
                    numBulletsPerShot = 3,
                    minSpreadBetweenShots = 40,
                },
                new CharacterStateComponent()
            }
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