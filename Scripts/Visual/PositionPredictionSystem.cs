using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CustomPhysics.Visual;

public partial class PositionPredictionSystem : Node
{
    [Export] private Line2D line;
    [Export] private PackedScene linePrefab;

    private List<Line2D> pool = new();
    private Dictionary<int, Line2D> inUse = new();

    private Line2D SpawnLine()
    {
        var line = linePrefab.Instantiate<Line2D>();
        AddChild(line);
        pool.Add(line);
        return line;
    }
    public override void _Process(double delta)
    {
        Dictionary<int, List<Vector2>> data = new();
        foreach (World world in PhysicsEngine.PredictionWorldData)
        {
            foreach (FakeRBData rb in world.FakeRbs)
            {
                if (!data.ContainsKey(rb.index))
                {
                    data.Add(rb.index, new List<Vector2>());
                }
                data[rb.index].Add(rb.position);
            }
        }
        
        foreach (KeyValuePair<int,List<Vector2>> pair in data)
        {
            if (!inUse.ContainsKey(pair.Key))
            {
                if (!pool.Any())
                {
                    SpawnLine();
                }

                var l = pool[0];
                pool.RemoveAt(0);
                inUse[pair.Key] = l;
            }
            inUse[pair.Key].ClearPoints();
            foreach (Vector2 vector2 in pair.Value)
            {
                inUse[pair.Key].AddPoint(vector2);
            }
        }
    }
}