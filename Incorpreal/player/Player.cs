using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        //Player will use WASD to move their character
        motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

         MoveAndCollide(motion.Normalized() * moveSpeed * delta);

        var collision = MoveAndCollide(motion.Normalized() * delta);


        if (collision != null)
        {
            if (collision.Collider.HasMethod("Hit"))
            {
                collision.Collider.Call("Hit");
            }
        }        
    }




}
