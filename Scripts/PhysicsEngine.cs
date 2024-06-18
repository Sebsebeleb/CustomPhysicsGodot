using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomPhysics.Scripts;

public partial class PhysicsEngine : Node
{

	private static PhysicsEngine instance;
	
	private World realWorld;

	private World worldBeingSimulated;
	
	private const float timeStep = 0.02f;

	private double timeStepCounter = 0;
	private double realTimeStepCounter = 0;

	private const int predictionSteps = 50;

	private bool dirty;

	public World[] PredictionWorlds;

	public static World[] PredictionWorldData => instance.PredictionWorlds;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		realWorld = worldBeingSimulated = new World();
		PredictionWorlds = new World[predictionSteps];
	}

	public static double GetFrameFraction()
	{
		return instance.timeStepCounter / timeStep;
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeStepCounter += delta * TimeControl.TimeScale;
		realTimeStepCounter += delta;
		bool simulated = false;
		while (timeStepCounter > timeStep)
		{
			simulated = true;
			timeStepCounter -= timeStep;
			ProcessPhysics(timeStep);
		}

		if (realTimeStepCounter > timeStep || simulated)
		{
			realTimeStepCounter = 0;
			PredictFuture();
		}

		if (!simulated)
		{
			return;
		}
	}


	public static void PredictFuturePlease()
	{
		instance.PredictFuture();
	}
	private void PredictFuture()
	{
		worldBeingSimulated = realWorld.Copy();
		for (int i = 0; i < predictionSteps; i++)
		{
			ProcessPhysics(timeStep);
			PredictionWorlds[i] = worldBeingSimulated.Copy();
		}
		// Done prediction, reset back to real world
		worldBeingSimulated = realWorld;
	}

	public static int SpawnAffector(AffectorData data)
	{
		return instance.DoSpawnAffector(data);
	}
	
	private int DoSpawnAffector(AffectorData data)
	{
		data.index = worldBeingSimulated.currentIndex++;
		worldBeingSimulated.Affectors.Add(data);

		return data.index;
	}

	public static World GetRealWorld()
	{
		return instance.realWorld;
	}
	
	public static int Spawn(FakeRBData data)
	{
		return instance.DoSpawn(data);
	}

	private int DoSpawn(FakeRBData data)
	{
		data.index = worldBeingSimulated.currentIndex++;
		worldBeingSimulated.FakeRbs.Add(data);

		return data.index;
	}
	
	private void ProcessPhysics(double delta){
		float minX = 0, minY = 0, maxX = 500, maxY = 500;
		var allRBs = worldBeingSimulated.FakeRbs;
		foreach (var rb in allRBs)
		{
			rb.position += rb.direction * rb.speed * (float)delta;

			if (rb.position.X - rb.widthOrRadius < minX)
			{
				rb.direction.X = Mathf.Abs(rb.direction.X);
			}
			if (rb.position.X + rb.widthOrRadius > maxX)
			{
				rb.direction.X = -Mathf.Abs(rb.direction.X);
			}
			if (rb.position.Y - rb.widthOrRadius < minY)
			{
				rb.direction.Y = Mathf.Abs(rb.direction.Y);
			}
			if (rb.position.Y + rb.widthOrRadius > maxY)
			{
				rb.direction.Y = -Mathf.Abs(rb.direction.Y);
			}
		}


		HashSet<(FakeRBData, FakeRBData)> handled = new HashSet<(FakeRBData, FakeRBData)>();
		foreach (var a in allRBs.ToArray())
		{
			foreach (var b in allRBs.ToArray())
			{
				if (a == b)
				{
					continue;
				}

				if (!handled.Add((a, b)))
				{
					continue;
				}

				var result = DetectCollision(a, b, out var penetrationDepth);
				if (result)
				{
					bool aCanBounce = a.bounces < 0 || a.bounces > 0;
					bool bCanBounce = b.bounces < 0 || b.bounces > 0;

					if (!aCanBounce || !bCanBounce)
					{
						if (a.bodyType == BODY_TYPE.Projectile)
						{
							//GD.Print("Remove body a: " + a);
							//allRBs.Remove(a);
						}
						else if (b.bodyType == BODY_TYPE.Projectile)
						{
							//GD.Print("Remove body b: " + b);
							//allRBs.Remove(b);
						}
					}
					Depenetrate(a, b, penetrationDepth);
					if (a.bodyType == BODY_TYPE.Projectile && b.bodyType == BODY_TYPE.Projectile)
					{
						var adir = a.direction;
						var aspeed = a.speed;
						a.direction = b.direction;
						a.speed = b.speed;
						b.direction = adir;
						b.speed = aspeed;
					}

					if ((a.bodyType == BODY_TYPE.Body && b.bodyType == BODY_TYPE.Projectile) ||
					    (b.bodyType == BODY_TYPE.Body && a.bodyType == BODY_TYPE.Projectile))
					{
						var body = a.bodyType == BODY_TYPE.Body ? a : b;
						var projectile = a.bodyType == BODY_TYPE.Projectile ? a : b;

						projectile.direction *= -1;
					}
				}
			}
		}

		foreach (FakeRBData rb in allRBs)
		{
			if (rb.bodyType != BODY_TYPE.Projectile)
			{
				continue;
			}
			foreach (AffectorData affector in worldBeingSimulated.Affectors)
			{
				if (OverlapCircle(rb.position, affector.position, affector.radius))
				{
					var direction = (rb.position - affector.position).Normalized();
					Vector2 drag = rb.direction + direction * affector.intensity;
					rb.direction = drag.Normalized();
				}

			}
		}

		for (var index = worldBeingSimulated.Affectors.Count - 1; index >= 0; index--)
		{
			var affector = worldBeingSimulated.Affectors[index];
			affector.remainingDuration -= (float)delta;
			if (affector.remainingDuration <= 0)
			{
				worldBeingSimulated.Affectors.RemoveAt(index);
			}
		}
	}

	private bool OverlapCircle(Vector2 a, Vector2 center, float radius)
	{
		return (a - center).Length() < radius;
	}
	private bool DetectCollision(FakeRBData a, FakeRBData b, out float penetrationDepth)
	{
		penetrationDepth = -1;
		var pointA = a.position;
		var pointB = b.position;
		var distance = (pointA - pointB).Length();

		//GD.Print($"Detected collision at: {pointA}({a.widthOrRadius}) from {a.index} against {pointB}({b.widthOrRadius}) from {b.index} with distance: {distance}");
		
		var diff = distance - (a.widthOrRadius + b.widthOrRadius);
		if (diff < 0)
		{
			penetrationDepth = diff/-2;
			return true;
		}
		else
		{
			return false;
		}
	}

	private void Depenetrate(FakeRBData a, FakeRBData b, float penetration)
	{
		var diff = (a.position - b.position).Normalized();
		//GD.Print("Depenetrate by: " + penetration);
		a.position += diff * penetration;
		b.position -= diff * penetration;
		/*a.position -= a.direction * penetration/2;
		b.position -= b.direction * penetration/2;*/
	}
}
