using Godot;
using System;

public class Bat : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        //Generally bats will not be controlled by the player but for sake of demonstration before enemy AI is added they will be controlled by the arrow keys. 
        motion.x = Input.GetActionStrength("move_rightTEMP") - Input.GetActionStrength("move_leftTEMP");
        motion.y = Input.GetActionStrength("move_downTEMP") - Input.GetActionStrength("move_upTEMP");

        MoveAndCollide(motion.Normalized() * moveSpeed * delta);
    }


    public void Hit()
    {
        Sprite playerSprite = (Sprite)GetNode("../Player/Sprite/player"); //Grab player sprite
        if (playerSprite.Texture.ResourcePath.Equals("res://assets/player.png")) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
            GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
            gp.enemyPath = GetPath();
            GetTree().ChangeScene("res://Battle.tscn");
        }
    }

}
