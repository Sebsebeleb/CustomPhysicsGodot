using System.Collections.Generic;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.AI;
using CustomPhysics.Gameplay.Character;
using CustomPhysics.Gameplay.Components;
using CustomPhysics.Gameplay.Tags;
using CustomPhysics.Gameplay.Weapons;
using Godot;

namespace CustomPhysics.Authoring;

public partial class AiAuth : Node2D
{
    public override void _Ready()
    {
        var BodyIndex = PhysicsEngine.Spawn(new FakeRBData()
        {
            position = this.GlobalPosition,
            bodyType = BODY_TYPE.Body,
            direction = Vector2.Zero,
            speed = 0,
            shapeType = 0,
            widthOrRadius = 32,
            height = 32,
            components = new List<IComponent>()
            {
                new VisualComponent()
                {
                    type = VisualComponent.VisualType.Sprite,
                    spriteID = 0,
                    size = new Vector2(32, 32),
                },
                new AiState()
                {
                    KnowsPlayerPosition = true,
                    desiredMinDistanceToPlayer = 200,
                    desiredMaxDistanceToPlayer = 500,
                },
                new MovementComponent()
                {
                    speed = 80
                },
                new CharacterStateComponent(),
                new WeaponComponent()
                {
                    StatAttackRate = 1,
                    numBulletsPerShot = 3,
                    minSpreadBetweenShots = Mathf.DegToRad(5f),
                }
            }
        });
    }
}