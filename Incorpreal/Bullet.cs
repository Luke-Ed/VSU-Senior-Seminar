using Godot;
using System;

public class Bullet : KinematicBody2D
{
    Vector2 velocity = new Vector2();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        velocity.y = -600;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Position.y < -1000)
        {
            QueueFree();
        }
        MoveAndSlide(velocity);

        var collision = MoveAndCollide(velocity.Normalized() * delta);

        if (collision != null)
        {
            if (collision.Collider.HasMethod("Hit"))
            {
                collision.Collider.Call("Hit");
                QueueFree();
            }
        }

    }
}
