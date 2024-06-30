using System.Linq;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using CustomPhysics.Physics;
using Godot;

namespace CustomPhysics.Gameplay.Systems;

[SystemType]
public class ConstantPushSystem : BaseSystem
{
    public override void Update(World world)
    {
	    return;
        foreach (FakeRBData rb in world.FakeRbs)
        {
            if (rb.position.X < 200)
            {
                rb.widthOrRadius = 4;
            }
            else if (rb.position.X > 400)
            {
                rb.widthOrRadius = 50;
            }
        }
        
    }

    public override void PhysicsProcess(World world)
    {
	    foreach (var query in world.GetComponentsByType<PushOrPullAffector>())
	    {
		    var comp = query.comp;
		    var id = query.entityId;
		    var affector = world.FakeRbs[id];

		    var nonEphemeralBodies = world.FakeRbs.Where(r => r.bodyType == BODY_TYPE.Body || r.bodyType == BODY_TYPE.Projectile);
		    foreach (var rb in nonEphemeralBodies)
		    {
			    if (rb.index == id)
			    {
				    continue;
			    }

			    if (PhysicsEngine.OverlapCircle(rb.position, affector.position, affector.widthOrRadius))
			    {
					var direction = (rb.position - affector.position).Normalized();
					Vector2 drag = rb.direction + direction * comp.intensity;
					rb.direction = drag.Normalized();
			    }
		    }
	    }
    }
}