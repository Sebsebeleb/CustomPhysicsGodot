using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Gameplay.Systems;

[SystemType]
public class MoveCharactersSystem : BaseSystem
{
    public override void PrePhysicsProcess(World world, float timestep)
    {
        foreach (var query in world.GetComponentsByType<MovementComponent>())
        {
            var entity = world.FakeRbs[query.entityId];
            var movementData = query.comp;
            if (!movementData.wantsToMove)
            {
                entity.speed = 0;
            }
            else
            {
                entity.speed = movementData.speed;
            }
            
            entity.direction = movementData.intendedDirection;
        }
    }
}