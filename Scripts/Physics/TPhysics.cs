using Godot;

namespace CustomPhysics.Physics;

public static class TPhysics
{
    public static bool CircleCircleCollision(Vector2 posA, Vector2 posB, float aWidth, float bWidth, out float penetration)
    {
	    penetration = -1;
		var distance = (posA - posB).Length();
		var diff = distance - (aWidth + bWidth);
		
		if (diff < 0)
		{
			penetration = diff/-2;
			return true;
		}
		else
		{
			return false;
		}
    }
}