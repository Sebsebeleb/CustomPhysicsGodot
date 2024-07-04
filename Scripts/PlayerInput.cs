using System.Collections.Generic;
using CustomPhysics.EcsEngine;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics;

public partial class PlayerInput : Node
{
    [Export] private PackedScene gravityWell;
    [Export] private Node2D player;

    private bool changingTime = false;
    public float targetFlowSpeed = 1f;

    private bool isPlacingAffector = false;
    private bool isShootPredicting = false;
    
    public override void _Ready()
    {
        PhysicsEngine.OnConstructPredictionWorld += OnPrediction;
    }

    public override void _ExitTree()
    {
        PhysicsEngine.OnConstructPredictionWorld -= OnPrediction;
    }

    private void OnPrediction()
    {
        if (isPlacingAffector)
        {
            var mbpos = GetViewport().GetMousePosition();
            PhysicsEngine.Spawn(new FakeRBData()
            {
                bodyType = BODY_TYPE.Ephemeral,
                widthOrRadius = 60.36f,
                position = mbpos,
                components = new List<IComponent>()
                {
                    new PushOrPullAffector()
                    {
                        duration = 7.115f,
                        intensity = 0.12f,
                    },
                    new VisualComponent()
                    {
                        color = new Color(0.7f, 0.3f, 0.8f, 0.5f),
                    },
                    new LifetimeComponent()
                    {
                        lifetime = 3f
                    }
                }
            });
        }

        if (isShootPredicting)
        {
            var mbpos = GetViewport().GetMousePosition();
            var playerPos = this.player.GlobalPosition;
            var direction = mbpos - playerPos;

            PhysicsEngine.Spawn(new FakeRBData()
            {
                position = playerPos,
                speed = 800,
                shapeType = 0,
                widthOrRadius = 6,
                height = 0,
                bounces = -1,
            });
        }
    }

    public override void _Input(InputEvent @event)
    {
        bool changedWorld = false;
        if (@event is InputEventMouseButton m && m.ButtonIndex == MouseButton.Right)
        {
            if (m.IsPressed())
            {
                isPlacingAffector = true;
            }

            if (m.IsReleased())
            {
                isPlacingAffector = false;
            }
        }
        
        if (@event is InputEventMouseButton mk && mk.ButtonIndex == MouseButton.Left)
        {
            if (mk.IsPressed())
            {
                isShootPredicting = true;
            }

            if (mk.IsReleased())
            {
                isShootPredicting = false;
            }
        }
        if (@event is InputEventMouseButton mb )
        {
            if (mb.ButtonIndex == MouseButton.Right && @event.IsReleased())
            {
                var mbpos = GetViewport().GetMousePosition();
                var well = gravityWell.Instantiate<Node2D>();
                well.GlobalPosition = mbpos;
                GetTree().Root.AddChild(well);
                changedWorld = true;
            }
            if (@event.IsPressed() && (mb.ButtonIndex == MouseButton.WheelUp || mb.ButtonIndex == MouseButton.WheelDown))
            {
                float delta = 0;
                if (mb.ButtonIndex == MouseButton.WheelUp)
                {
                    delta += 0.015f;
                }
                else if (mb.ButtonIndex == MouseButton.WheelDown)
                {
                    delta -= 0.015f;
                }

                float min = 0;
                float max = 1f;
                
                var current = TimeControl.TimeScale;
                current += delta;

                current = Mathf.Clamp(current, min, max);
                TimeControl.TimeScale = current;
            }
        }

        if (@event.IsActionPressed("StopTime"))
        {
            changingTime = true;
            targetFlowSpeed = 0f;
        }

        if (@event.IsActionPressed("SetToRegularFlow"))
        {
            changingTime = true;
            targetFlowSpeed = 1f;
        }

        if (changedWorld)
        {
            PhysicsEngine.PredictFuturePlease();
        }
    }

    public override void _Process(double delta)
    {
        if (changingTime)
        {
            var current = TimeControl.TimeScale;
            var sign = current < targetFlowSpeed ? 1 : -1;
            var bonusMultiplier = sign < 0 ? 5 : 1;
            current += (float)delta * 2 * Mathf.Max(current, 0.1f) * sign * bonusMultiplier;

            current = Mathf.Clamp(current, 0, 1);
            TimeControl.TimeScale = current;

            if (sign > 0 && current > targetFlowSpeed)
            {
                current = targetFlowSpeed;
            }
            else if (sign < 0 && current < targetFlowSpeed)
            {
                current = targetFlowSpeed;
            }

            if (Mathf.IsEqualApprox(targetFlowSpeed, current))
            {
                changingTime = false;
            }
        }
    }
}