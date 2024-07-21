using System.Collections.Generic;
using System.Linq;
using CustomPhysics.Gameplay.Components;
using Godot;

namespace CustomPhysics.Utility;

public partial class EcsRenderer : Node2D
{
    private World oldWorld;
    private List<World> queuedData = new List<World>();

    private EcsRendererResources resourceData;
    
    public void SetData(World world)
    {
        if (this.queuedData.Count > 0)
        {
            oldWorld = this.queuedData[0];
        }
        this.queuedData.Clear();
        this.queuedData.Add(world.Copy());
        //this.QueueRedraw();
    }
    
    public override void _Ready()
    {
        resourceData = GD.Load<EcsRendererResources>("res://Data/Visual/EcsRendererResource.tres");
    }

    public override void _Process(double delta)
    {
        this.QueueRedraw();
    }

    public override void _Draw()
    {
        /*if (!this.queuedData.Any())
        {
            return;
        }*/
        
        if (DebugSettings.ShowBodyPredictions)
        {
            DrawPredictionBodyPositions();
        }
        DrawEntities();
        DrawViewCones();

        //this.queuedData.Clear();
    }

    private void DrawViewCones()
    {
        // Only uses the "Real time" world (TODO: Note that [0] is probably not actually the real simulated world atm
        var world = queuedData[0];

        foreach ((ViewConeComponent comp, int entityId, int indexInner) tuple in world.GetComponentsByType<ViewConeComponent>())
        {
            var cone = tuple.comp;
            var rb = world.GetRBById(tuple.entityId);
            var points = Drawing.Drawing.GetArcPolygonPoints(rb.position, 250, 0, 60, 20);
            points.Add(rb.position);
            points.Insert(0, rb.position);
            var asArray = points.ToArray();
            var fillColor = new Color(0.698F, 0.301f, 0.301f, 0.5f);
            var outlineColor = new Color(0.925F, 0.145F, 0.145F, 0.6f);
            DrawPolygon(asArray, new [] { fillColor });
            DrawPolyline(asArray, outlineColor, width:3, antialiased:true);
        }
    }

    private void DrawPredictionBodyPositions()
    {
        for (var index = 0; index < PhysicsEngine.PredictionWorldData.Length; index++)
        {
            var world = PhysicsEngine.PredictionWorldData[index];
            
            var query = world.GetComponentsByType<VisualComponent>();
            foreach (var result in query)
            {
                var rb = world.GetRBById(result.entityId);
                Color color;
                if (DebugSettings.AlternateBodyPredictionColors)
                {
                    color = index % 2 == 0 ? new Color(1, 0.3f, 0.3f, 0.4f) : new Color(0.3f, 1f, 0.3f, 0.4f);
                }
                else
                {
                    color = new Color(1, 1, 1, 0.2f);
                }
                DrawArc(rb.position-GlobalPosition, rb.widthOrRadius, 0, 360, 24, color, 1.5f);
            }
        }
    }

    private void DrawEntities()
    {
        foreach (var world in queuedData)
        {
            var query = world.GetComponentsByType<VisualComponent>();
            foreach (var result in query)
            {
                var vComp = result.comp;
                var id = result.entityId;
                var rb = world.FakeRbs.FirstOrDefault(r => r.index == id);

                var rbPosition = rb.position;
                Vector2 previousPosition = rb.position;
                
                // Check if we have a reference point for previous position
                if (oldWorld != null)
                {
                    // Ensure this entity also existed in the old world
                    var oldEntity = oldWorld.GetRBById(id);
                    if (oldEntity != null)
                    {
                        previousPosition = oldEntity.position;
                        double frameFraction = PhysicsEngine.GetFrameFraction();
                        
                        rbPosition = previousPosition.Lerp(rbPosition, (float)frameFraction);
                    }
                }
                if (vComp.type == VisualComponent.VisualType.Shape)
                {
                    DrawCircle(rbPosition, rb.widthOrRadius, vComp.color);
                }

                if (vComp.type == VisualComponent.VisualType.Sprite)
                {
                    var textureExists = this.resourceData.TryGetTextureById(vComp.spriteID, out var tex);
                    if (!textureExists)
                    {
                        GD.PrintErr("Failed to find sprite with id: " + vComp.spriteID);
                        continue;
                    }
                    DrawTextureRect(tex, new Rect2(rbPosition - vComp.size/2, vComp.size),false, vComp.color);
                }
            }
        }
    }
}