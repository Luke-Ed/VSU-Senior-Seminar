using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;
    public CollisionShape2D hitbox;
    public AnimationPlayer animator;

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        animator = (AnimationPlayer)GetNode("AnimationPlayer");
        motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        //MoveAndCollide(motion.Normalized() * moveSpeed * delta);

        var collision = MoveAndCollide(motion.Normalized() * moveSpeed * delta);
        if (collision != null) {

        }
    }

    public void change_state(string new_state) {
        switch (new_state) {
            case "ready":
                animator.Play("Idle");
                break;
            case "walking":
                animator.Play("Walking");
                break;
            case "dead":
                animator.Play("Die");
                break;
            default:
                break;
        }
    }
}
