using CustomPhysics.EcsEngine;

namespace CustomPhysics.Gameplay.Components;

public struct ViewConeComponent : IComponent
{
    public float angleOffset;
    public float width;
    public float length;
}