namespace CustomPhysics.EcsEngine;

public abstract class BaseSystem
{

    public virtual void Initialize() { }

    public virtual void OnOverlap(FakeRB a, FakeRB b) { }

    public static void GetComponentsFor<T>(int id){ }

    public virtual void Update(World world) { }

    public virtual void PhysicsProcess(World world) { }
}