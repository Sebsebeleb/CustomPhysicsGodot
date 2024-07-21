using CustomPhysics.EcsEngine;

namespace CustomPhysics.Gameplay.Weapons;

public struct WeaponComponent : IComponent
{
    public enum FiringMethod
    {
        Bolt,
        //Burst,
        //Automatic,
    }

    public enum SpreadMethod
    {
        FixedDifference,
    }

    public FiringMethod FireMethod = FiringMethod.Bolt;
    public SpreadMethod Spread = SpreadMethod.FixedDifference;
    
    public float StatAttackRate = 0;
    public float Cooldown = 0;
    public bool BeingFired = false;
    
    // Related to fire method
    public int numBulletsPerShot = 1;
    public float minSpreadBetweenShots = 0;
    public float maxSpreadBetweenShots = 0;

    public WeaponComponent()
    {
    }
}