using Godot;
using System;

public class MiniGamePlayer : KinematicBody2D
{
    Node HitTargetNode;
    Timer timer;
    Boolean onCooldown = false;
    ColorRect targetPage;

    public override void _Ready()
    {
        HitTargetNode = GetParent();
        timer = HitTargetNode.GetNode<Timer>("ShotCooldown");
        timer.WaitTime = 1;
        timer.Connect("timeout", this, "onTimeout");
        targetPage = (ColorRect)GetParent();
    }

    public override void _PhysicsProcess(float delta)
    {
        int moveSpeed = 125;
        var motion = new Vector2();
        //Player will use WASD to move their character
        motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        /*
        if (!motion.x.Equals(0) || !motion.y.Equals(0)) { //For Player animations
                ChangeState("Walking");
            } else {
                ChangeState("Idle");
            }
        */
        MoveAndCollide(motion.Normalized() * moveSpeed * delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("shoot"))
        {
            if (!onCooldown && targetPage.Visible == true)
            {
                PackedScene projectile = (PackedScene)ResourceLoader.Load("res://Bullet.tscn");
                KinematicBody2D bullet = (KinematicBody2D)projectile.Instance();
                Vector2 bulletPosition = this.Position;
                bulletPosition.x += 15;
                bullet.Position = bulletPosition;
                GetTree().CurrentScene.AddChild(bullet);
                onCooldown = true;
                timer.Start();
            }
        }
    }

    public void onTimeout()
    {
        onCooldown = false;
    }
}
