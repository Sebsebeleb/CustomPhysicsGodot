using CustomPhysics.EcsEngine;

namespace CustomPhysics.Gameplay.Components;

public struct PushOrPullAffector : IComponent
{
    public float intensity;
    public float duration;
}