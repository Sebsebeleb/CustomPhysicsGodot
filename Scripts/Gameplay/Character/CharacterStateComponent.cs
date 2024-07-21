using IComponent = CustomPhysics.EcsEngine.IComponent;

namespace CustomPhysics.Gameplay.Character;

public struct CharacterStateComponent : IComponent
{
    public bool WantsToShoot;
}