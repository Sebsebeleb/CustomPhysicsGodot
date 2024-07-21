using System;
using System.Collections.Generic;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Gameplay.Weapons;

[SystemType]
public class FireWeaponsSystem : BaseSystem
{
    public const int maxFutureStepPrediction = 2;
    public override void PhysicsProcess(World world, float timestep)
    {
        if (PhysicsEngine.StepsIntoTheFuture >= maxFutureStepPrediction)
        {
            return;
        }
        foreach ((WeaponComponent comp, int entityId, int indexInner) result in world
                     .GetComponentsByType<WeaponComponent>())
        {
            var weaponComponent = result.comp;
            var rb = world.FakeRbs[result.entityId];
            if (!weaponComponent.BeingFired || weaponComponent.Cooldown > 0)
            {
                continue;
            }

            weaponComponent.Cooldown = weaponComponent.StatAttackRate;
            world.ComponentLookup[result.entityId][result.indexInner] = weaponComponent;

            // Fire!

            var currentAngle = Mathf.Atan2(rb.direction.Y, rb.direction.X);
            var numShots = weaponComponent.numBulletsPerShot;
            if (numShots > 1)
            {
                if (numShots % 2 == 1)
                {
                    Fire(rb.position, currentAngle, rb.widthOrRadius*1.1f);
                    numShots--;
                }

                for (int i = -numShots / 2; i <= numShots / 2; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    Fire(rb.position, currentAngle + i * weaponComponent.minSpreadBetweenShots, rb.widthOrRadius*1.1f);
                }
            }
        }
    }

    private static void Fire(Vector2 position, float angle, float offset)
    {
        var direction = Vector2.FromAngle(angle);
        PhysicsEngine.Spawn(new FakeRBData()
        {
            position = position + direction * offset * 1.1f,
            direction = direction,
            speed = 800,
            bodyType = BODY_TYPE.Projectile,
            shapeType = 0,
            widthOrRadius = 6,
            height = 0,
            bounces = 3,
            components = new()
            {
                new VisualComponent()
                {
                    size = new Vector2(6, 6),
                    color = Colors.OrangeRed,
                    type = VisualComponent.VisualType.Shape
                }
            }
        });
    }
}