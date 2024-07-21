using CustomPhysics.EcsEngine;
using Godot;

namespace CustomPhysics.Gameplay.Components;


public struct MovementComponent : IComponent
{
    public Vector2 intendedDirection;
    public bool wantsToMove;
    public float speed;
}