using System.Collections.Generic;
using Godot;

namespace CustomPhysics.Scripts;

public class World
{
    public List<FakeRBData> FakeRbs = new List<FakeRBData>();

    public List<AffectorData> Affectors = new List<AffectorData>();

    public int currentIndex = 0;
    
    public World Copy()
    {
        var w2 = new World();
        foreach (FakeRBData rb in FakeRbs)
        {
            w2.FakeRbs.Add(new FakeRBData()
            {
                position = rb.position,
                bodyType =  rb.bodyType,
                direction = rb.direction,
                height = rb.height,
                index = rb.index,
                shapeType = rb.shapeType,
                speed = rb.speed,
                widthOrRadius = rb.widthOrRadius,
                bounces = rb.bounces,
            });
        }

        foreach (AffectorData data in Affectors)
        {
            w2.Affectors.Add(new AffectorData()
            {
                intensity = data.intensity,
                position = data.position,
                radius = data.radius,
                remainingDuration = data.remainingDuration
            });
        }

        return w2;
    }
}

public class AffectorData
{
    public int index;
    public Vector2 position;
    public float radius;
    public float intensity;
    public float remainingDuration;
}