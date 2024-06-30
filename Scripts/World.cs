using System.Collections.Generic;
using System.Linq;
using CustomPhysics.EcsEngine;
using Godot;

namespace CustomPhysics;

public class World
{
    public List<FakeRBData> FakeRbs = new ();
    public Dictionary<int, List<Component>> ComponentLookup = new ();

    public int currentIndex = 0;

    /// <summary>
    /// Given a type of component, returns all of them, and the entity they are attached to
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<(T comp, int entityId)> GetComponentsByType<T>()
    {
        List<(T comp, int i)> results = new List<(T comp, int i)>();
        foreach (var pair in ComponentLookup)
        {
            var id = pair.Key;
            foreach (var comp in pair.Value)
            {
                if (comp is T c)
                {
                    results.Add((c, id));
                }
            }
        }

        return results;
    }
    
    public World Copy()
    {
        var w2 = new World();
        
        // Copy index
        w2.currentIndex = this.currentIndex;
        
        // Copy RBS
        foreach (FakeRBData rb in FakeRbs)
        {
            var copyComponents = new Component[rb.components.Count];
            rb.components.CopyTo(copyComponents);

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
                components = copyComponents.ToList(),
            });
        }
        
        // Copy ComponentLookups
        w2.ComponentLookup = new Dictionary<int, List<Component>>();
        foreach (KeyValuePair<int,List<Component>> keyValuePair in ComponentLookup)
        {
            var data = new Component[this.ComponentLookup[keyValuePair.Key].Count];
            keyValuePair.Value.CopyTo(data);
            w2.ComponentLookup[keyValuePair.Key] = data.ToList();
        }
        
        
        // TODO: Maybe use IClonable and MemberwiseClone instead? should work?
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
    public Component[] components;
}