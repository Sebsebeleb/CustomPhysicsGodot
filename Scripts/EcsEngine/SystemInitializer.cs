using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace CustomPhysics.EcsEngine;

public partial class SystemInitializer : Node
{
    private List<BaseSystem> Systems = new ();
    public override void _EnterTree()
    {
        Systems.Clear();
        var asm = this.GetType().Assembly;

        foreach (var t in asm.GetTypes())
        {
            var attr = t.GetCustomAttribute<SystemTypeAttribute>();
            var disabled = t.GetCustomAttribute<DisabledSystem>();
            if (attr == null || disabled != null) continue;

            var instance = Activator.CreateInstance(t) as BaseSystem;
            instance.Initialize();
            Systems.Add(instance);
        }
        
        PhysicsEngine.OnPhysicsProcess += PhysicsEngineOnOnPhysicsProcess;
        PhysicsEngine.PrePhysicsProcess += PhysicsEngineOnPrePhysicsProcess;
        PhysicsEngine.OnRealWorldStepped += PhysicsEngineOnOnRealWorldStepped;
    }

    private void PhysicsEngineOnOnRealWorldStepped(World world)
    {
        foreach (BaseSystem system in Systems)
        {
            system.OnRealWorldStep(world);
        }
    }

    private void PhysicsEngineOnPrePhysicsProcess(World world, float timestep)
    {
        foreach (BaseSystem system in Systems)
        {
            system.PrePhysicsProcess(world, timestep);
        }
    }

    private void PhysicsEngineOnOnPhysicsProcess(World world, float timestep)
    {
        foreach (BaseSystem system in Systems)
        {
            system.PhysicsProcess(world, timestep);
        }
        
        world.ClearDeadEntities();
    }

    public override void _Process(double delta)
    {
        foreach (var system in Systems)
        {
            system.Update(PhysicsEngine.GetRealWorld(), (float)delta);
        }
    }
}