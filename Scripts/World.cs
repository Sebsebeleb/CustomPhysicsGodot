using System;
using System.Collections.Generic;
using System.Linq;
using CustomPhysics.EcsEngine;
using Godot;

namespace CustomPhysics;

public record World
{
    public List<FakeRBData> FakeRbs = new ();
    public Dictionary<int, IComponent[]> ComponentLookup = new ();
    

    public int currentIndex = 0;

    private int currentComponentIndex = 0;

    /// <summary>
    /// Given a type of component, returns all of them, and the entity they are attached to
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<(T comp, int entityId, int indexInner)> GetComponentsByType<T>() where T: IComponent
    {
        List<(T comp, int entityId, int indexInner)> results = new ();

        foreach (var pair in ComponentLookup)
        {
            var entityId = pair.Key;
            var components = ComponentLookup[entityId];
            
            for (var indexInner = 0; indexInner < components.Length; indexInner++)
            {
                var comp = components[indexInner];
                if (comp is T c)
                {
                    results.Add((c, entityId, indexInner));
                }
            }
        }

        return results;
    }

    
    public World Copy()
    {
        /*
        var w2 = this.MemberwiseClone() as World;
        return w2;
        */
        var w2 = new World();
        
        // Copy index
        w2.currentIndex = this.currentIndex;
        
        // Copy RBS
        foreach (FakeRBData rb in FakeRbs)
        {
            var copyComponents = new IComponent[rb.components.Count];
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
        w2.ComponentLookup = new Dictionary<int, IComponent[]>();
        foreach (var pair in ComponentLookup)
        {
            var data = new IComponent[this.ComponentLookup[pair.Key].Length];
            pair.Value.CopyTo((Memory<IComponent>)data);
            w2.ComponentLookup[pair.Key] = data.ToArray();
        }
        
        
        // TODO: Maybe use IClonable and MemberwiseClone instead? should work?
        return w2;
    }

    public void ClearDeadEntities()
    {
        foreach (var fakeRb in this.FakeRbs.ToArray())
        {
            if (fakeRb.dead)
            {
                DeleteEntity(fakeRb.index);
            }
        }
    }

    public void DeleteEntity(int id)
    {
        GD.Print($"Delete ID {id}");
        var componentsHasId = ComponentLookup.ContainsKey(id);
        var entity = this.FakeRbs.FirstOrDefault(rb => rb.index == id);

        if (!componentsHasId || entity == null)
        {
            GD.PrintErr($"Asked to delete non-existing id: {id}, {componentsHasId}, {entity == null}");
        }

        this.FakeRbs.Remove(entity);
        var removed = this.ComponentLookup.Remove(id);
        if (!removed)
        {
            GD.PrintErr($"Huh? failed to remove component thing: {id}");
        }
    }

    public void MarkForDeletion(int id)
    {
        var entity = this.FakeRbs.FirstOrDefault(rb => rb.index == id);
        if (entity == null)
        {
            GD.PrintErr("Was asked to mark invalid id for destruction: " +id);
            return;
        }
        entity.dead = true;
    }
}