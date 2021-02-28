using Godot;
using System;

public class Bat : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;
    public int attack = 2;
    public int health = 30;
    public int currentHealth = 30;
    public string name = "bat";
    public GlobalPlayer gp;

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        //Generally bats will not be controlled by the player but for sake of demonstration before enemy AI is added they will be controlled by the arrow keys. 
        motion.x = Input.GetActionStrength("move_rightTEMP") - Input.GetActionStrength("move_leftTEMP");
        motion.y = Input.GetActionStrength("move_downTEMP") - Input.GetActionStrength("move_upTEMP");

        MoveAndCollide(motion.Normalized() * moveSpeed * delta);
    }

    public override void _Ready()
    {
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    public void playTurn()
    {
        gp.takeDamage(attack);
    }


    public void Hit()
    {
        NodePath np = GetPath();
        gp.nodePaths.Add(np);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        var newNode = Duplicate();
        tq.AddChild(newNode);
        newNode.QueueFree();
        GetTree().ChangeScene("res://Battle.tscn");
    }

}
