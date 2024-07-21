using CustomPhysics.EcsEngine;

namespace CustomPhysics.Gameplay.Weapons;

[SystemType]
public class CooldownWeaponsSystem : BaseSystem
{
    public override void PhysicsProcess(World world, float timestep)
    {
        foreach (var result in world.GetComponentsByType<WeaponComponent>())
        {
            var comp = result.comp;
            var rbId = result.entityId;

            comp.Cooldown -= timestep;
            world.ComponentLookup[rbId][result.indexInner] = comp;
        }
    }
}