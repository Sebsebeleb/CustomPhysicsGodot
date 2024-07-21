using System.Linq;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Character;
using CustomPhysics.Gameplay.Components;
using CustomPhysics.Gameplay.Tags;
using Godot;

namespace CustomPhysics.Gameplay.AI;

[SystemType]
public class AIUpdateSystem : BaseSystem
{
    public override void PrePhysicsProcess(World world, float timestep)
    {
        var found = world.GetComponentsByType<AiState, MovementComponent, CharacterStateComponent>();
        foreach (var query in found)
        {
            var rb = world.GetRBById(query.entityId);
            var aiComponent = query.compA;
            var movementComponent = query.compB;
            var characterStateComponent = query.compC;
            var movementIndex = query.indexInnerB;
            var characterStateIndex = query.indexInnerC;

            if (aiComponent.KnowsPlayerPosition)
            {
                var players = world.GetComponentsByType<PlayerTag>();
                if (players.Count != 1)
                {
                    GD.PrintErr($"Eitehr didnt find the player, or there were too many: {players.Count}");
                }
                var player = players.FirstOrDefault();
                var playerRB = world.GetRBById(player.entityId);
                var diff = (playerRB.position - rb.position);

                var direction = diff.Normalized();
                bool wantsToMove = true;

                var distance = diff.Length();
                // Too far away, move towards player
                if (distance > aiComponent.desiredMaxDistanceToPlayer)
                {
                    
                }
                // Too close, go away
                else if (distance < aiComponent.desiredMinDistanceToPlayer)
                {
                    direction = direction *= -1;
                }
                // Good distance. Stay still
                else
                {
                    wantsToMove = false;
                }

                
                movementComponent.intendedDirection = direction;
                movementComponent.wantsToMove = wantsToMove;
                world.ComponentLookup[query.entityId][movementIndex] = movementComponent;
            }

            characterStateComponent.WantsToShoot = aiComponent.KnowsPlayerPosition;
            world.ComponentLookup[query.entityId][characterStateIndex] = characterStateComponent;
        }
    }
}