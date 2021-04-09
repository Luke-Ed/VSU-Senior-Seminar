using Godot;
using System;

public class Path2D : Godot.Path2D
{
    public PathFollow2D follow;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        follow = GetNode<PathFollow2D>("PathFollow2D");
        SetProcess(true);
    }

    public override void _Process(float delta)
    {
        if (follow.GetChildCount() > 0)
        {
            Node enemy = follow.GetChild(0);
            if (enemy.Get("moveSpeed") != null)
            {
                follow.Offset = (follow.Offset + (int)enemy.Get("moveSpeed") * delta);
            }
        }
    }
}