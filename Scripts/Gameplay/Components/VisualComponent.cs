using CustomPhysics.EcsEngine;
using Godot;

namespace CustomPhysics.Gameplay.Components;

public struct VisualComponent : IComponent
{
    public enum VisualType
    {
        Shape,
        Sprite
    }
    public Color color = Colors.White;
    public VisualType type = VisualType.Shape;
    public int spriteID = -1;
    public Vector2 size = default;

    public VisualComponent()
    {
    }
}