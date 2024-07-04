namespace CustomPhysics.EcsEngine;

public abstract class BaseSystem
{

    public virtual void Initialize() { }

    public virtual void OnOverlap(FakeRB a, FakeRB b) { }

    public static void GetComponentsFor<T>(int id){ }

    /// <summary>
    /// Called during regular node's update
    /// </summary>
    /// <param name="world"></param>
    /// <param name="timestep"></param>
    public virtual void Update(World world, float timestep) { }

    /// <summary>
    /// Called every time the physics engine processes the "physics" (more like whole game step atm)
    /// </summary>
    /// <param name="world"></param>
    /// <param name="timestep"></param>
    public virtual void PhysicsProcess(World world, float timestep) { }
}