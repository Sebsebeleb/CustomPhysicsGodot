using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Weapons;
using Godot;

namespace CustomPhysics.Gameplay.Character;

[SystemType]
public class FireWeaponsForCharactersSystem : BaseSystem
{
    public override void PhysicsProcess(World world, float timestep)
    {
        foreach (var result in world.GetComponentsByType<CharacterStateComponent, WeaponComponent>())
        {
            var characterComp = result.compA;
            var weaponComp = result.compB;

            weaponComp.BeingFired = characterComp.WantsToShoot;
            world.ComponentLookup[result.entityId][result.indexInnerB] = weaponComp;
        }
    }
}