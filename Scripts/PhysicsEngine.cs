using System;
using System.Collections.Generic;
using System.Linq;
using CustomPhysics.EcsEngine;
using CustomPhysics.Physics;
using Godot;

namespace CustomPhysics;

public partial class PhysicsEngine : Node
{

	public static event Action OnConstructPredictionWorld;
	
	/// <summary>
	/// This is called once every time the REAL world has been properly stepped forward in time.
	/// </summary>
	public static event Action<World> OnRealWorldStepped;
	public static event Action<World, float> OnPhysicsProcess;
	public static event Action<World, float> PrePhysicsProcess;
	
	private static PhysicsEngine instance;
	
	private World realWorld;

	private World worldBeingSimulated;
	private int stepsIntoTheFuture = 0;
	public static int StepsIntoTheFuture => instance.stepsIntoTheFuture;

	/// <summary>
	/// The number of seconds we can see into the future. Affects the timeStep used for physics calculation in combination with <see cref="predictionSteps"/>
	/// </summary>
	private float secondsPredicted = 4.5f;
	public static float timeStep => instance.secondsPredicted/instance.predictionSteps;

	private double timeStepCounter = 0;
	private double realTimeStepCounter = 0;

	private int predictionSteps = 300;

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

	/// <summary>
	/// Temp for debug. Needed function maybe, but im thinking "is real" can be a property on the world class 
	/// </summary>
	/// <param name="world"></param>
	/// <returns></returns>
	public static bool IsRealWorld(World world)
	{
		return world == instance.realWorld;
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

		if (simulated)
		{
			OnRealWorldStepped?.Invoke(realWorld);
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


	public static void SetPredictionSteps(int nextNumSteps)
	{
		instance.predictionSteps = nextNumSteps;
	}
	
	public static void SetPredictionTime(float timePredicted)
	{
		instance.secondsPredicted = timePredicted;
	}

	public static void PredictFuturePlease()
	{
		instance.PredictFuture();
	}
	private void PredictFuture()
	{
		worldBeingSimulated = realWorld.Copy();
		
		// Let other systems spawn things etc.
		OnConstructPredictionWorld?.Invoke();
		
		for (int i = 0; i < predictionSteps; i++)
		{
			stepsIntoTheFuture = i;
			ProcessPhysics(timeStep);
			worldBeingSimulated = worldBeingSimulated.Copy();
			PredictionWorlds[i] = worldBeingSimulated;
		}

		stepsIntoTheFuture = 0;
		// Done prediction, reset back to real world
		worldBeingSimulated = realWorld;
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
		worldBeingSimulated.ComponentLookup[data.index] = new IComponent[data.components.Count];
		data.components.CopyTo(worldBeingSimulated.ComponentLookup[data.index]);

		return data.index;
	}
	
	private void ProcessPhysics(double delta){
		float minX = 0, minY = 0, maxX = 1400, maxY = 850;


		PrePhysicsProcess?.Invoke(worldBeingSimulated, (float)delta);
		
		var allRBs = worldBeingSimulated.FakeRbs;
		foreach (var rb in allRBs)
		{
			rb.position += rb.direction * rb.speed * (float)delta;

			if (rb.position.X - rb.widthOrRadius < minX)
			{
				rb.bounces--;
				rb.direction.X = Mathf.Abs(rb.direction.X);
			}
			if (rb.position.X + rb.widthOrRadius > maxX)
			{
				rb.bounces--;
				rb.direction.X = -Mathf.Abs(rb.direction.X);
			}
			if (rb.position.Y - rb.widthOrRadius < minY)
			{
				rb.bounces--;
				rb.direction.Y = Mathf.Abs(rb.direction.Y);
			}
			if (rb.position.Y + rb.widthOrRadius > maxY)
			{
				rb.bounces--;
				rb.direction.Y = -Mathf.Abs(rb.direction.Y);
			}

			if (rb.bodyType == BODY_TYPE.Projectile && rb.bounces <= 0)
			{
				worldBeingSimulated.MarkForDeletion(rb.index);
			}
		}


		HashSet<(FakeRBData, FakeRBData)> handled = new HashSet<(FakeRBData, FakeRBData)>();
		foreach (var a in allRBs.Where(r => r.bodyType != BODY_TYPE.Ephemeral))
		{
			foreach (var b in allRBs.Where(r => r.bodyType != BODY_TYPE.Ephemeral))
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
							worldBeingSimulated.MarkForDeletion(a.index);
							//GD.Print("Remove body a: " + a);
							//allRBs.Remove(a);
						}
						else if (b.bodyType == BODY_TYPE.Projectile)
						{
							worldBeingSimulated.MarkForDeletion(b.index);
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


		OnPhysicsProcess?.Invoke(worldBeingSimulated, (float)delta);
	}

	public static bool OverlapCircle(Vector2 a, Vector2 center, float radius)
	{
		return (a - center).Length() < radius;
	}
	public static bool DetectCollision(FakeRBData a, FakeRBData b, out float penetrationDepth)
	{
		penetrationDepth = -1;
		var pointA = a.position;
		var pointB = b.position;

		var widthA = a.widthOrRadius;
		var widthB = b.widthOrRadius;

		return TPhysics.CircleCircleCollision(pointA, pointB, widthA, widthB, out penetrationDepth);
	}

	public static void Depenetrate(FakeRBData a, FakeRBData b, float penetration)
	{
		var diff = (a.position - b.position).Normalized();
		//GD.Print("Depenetrate by: " + penetration);
		a.position += diff * penetration;
		b.position -= diff * penetration;
		/*a.position -= a.direction * penetration/2;
		b.position -= b.direction * penetration/2;*/
	}
}