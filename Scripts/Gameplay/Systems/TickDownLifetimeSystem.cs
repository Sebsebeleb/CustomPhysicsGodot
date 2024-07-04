using System.Collections.Generic;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Gameplay.Systems;


[SystemType]
public class TickDownLifetimeSystem : BaseSystem
{
    public override void PhysicsProcess(World world, float timestep)
    {
        bool isRealWorld = PhysicsEngine.IsRealWorld(world);
        List<int> toDelete = new();
        
        
        
        
        
        /*
        world.Apply<LifetimeComponent>((component, index) =>
        {
            component.lifetime -= timestep;
            GD.Print($"{component.lifetime} (-{timestep}) ({isRealWorld})");
            if (component.lifetime <= 0)
            {
                toDelete.Add(index);
            }
        });*/
        
        
        
        
        var res = world.GetComponentsByType<LifetimeComponent>();
        for (var index = 0; index < res.Count; index++)
        {
            var query = res[index];
            var entityId = query.entityId;
            var innerIndex = query.indexInner;
            var life = query.comp;
            life.lifetime -= timestep;
            //query.comp.lifetime = 2;
            if (life.lifetime <= 0)
            {
                toDelete.Add(query.entityId);
            }

            world.ComponentLookup[entityId][innerIndex] = life;
        }
        foreach (var i in toDelete)
        {
            world.MarkForDeletion(i);
        }
    }
}