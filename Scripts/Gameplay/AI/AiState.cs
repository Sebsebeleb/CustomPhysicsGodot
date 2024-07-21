using CustomPhysics.EcsEngine;

namespace CustomPhysics.Gameplay.AI;

public struct AiState : IComponent
{
    public bool KnowsPlayerPosition = false;
    public float desiredMaxDistanceToPlayer = -1;
    public float desiredMinDistanceToPlayer = -1;

    public AiState()
    {
    }
}