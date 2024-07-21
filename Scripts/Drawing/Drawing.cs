using System;
using System.Collections.Generic;
using Godot;

namespace CustomPhysics.Drawing;

public static class Drawing
{
    public static List<Vector2> GetArcPolygonPoints(Vector2 center, float radius, float startAngle, float endAngle,
        int segments)
    {
        List<Vector2> points = new List<Vector2>();

        // Convert angles from degrees to radians
        float startRadians = (float)(startAngle * Math.PI / 180);
        float endRadians = (float)(endAngle * Math.PI / 180);

        // Calculate the angle increment
        float angleIncrement = (endRadians - startRadians) / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = startRadians + i * angleIncrement;
            float x = center.X + radius * (float)Math.Cos(angle);
            float y = center.Y + radius * (float)Math.Sin(angle);
            points.Add(new Vector2(x, y));
        }

        return points;
    }
}