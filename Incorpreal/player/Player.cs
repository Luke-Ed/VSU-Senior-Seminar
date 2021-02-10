using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;
    public CollisionShape2D hitbox;
    public AnimationPlayer animate;

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        animate = (AnimationPlayer)GetNode("AnimationPlayer");
        motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        //MoveAndCollide(motion.Normalized() * moveSpeed * delta);

        var collision = MoveAndCollide(motion.Normalized() * moveSpeed * delta);
        if (collision != null) {
            animate.Play("Die");            
        }
    }

    public void change_state(string new_state) {
        switch (new_state) {
            case "ready":
                animate.Play("Idle");
                break;
            case "dead":
                animate.Play("Die");
                break;
            default:
                break;
        }
    }
}
