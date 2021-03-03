using Godot;
using System;

public class Bat : KinematicBody2D
{
    [Export]

    public int moveSpeed = 50;
    public int attack;
    public int health;
    public int currentHealth;
    public string enemyName;
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
        var filePath = "res://Enemies/enemies.txt";
        File newFile = new File();
        newFile.Open(filePath, File.ModeFlags.Read);
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        while (!newFile.EofReached())
        {
            String s = newFile.GetLine();
            if (Name.Contains(s)){
                enemyName = s;
                attack = int.Parse(newFile.GetLine());
                health = int.Parse(newFile.GetLine());
                currentHealth = health;
            }
        }
    }

    public Boolean playTurn()
    {
        if (!gp.isDefending)
        {
            return gp.takeDamage(attack);
        }
        else
        {
            return gp.takeDamage(attack / 2);
        }
    }


    public void Hit()
    {
        NodePath np = GetPath();
        gp.nodePaths.Add(np);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).Name = enemyName;
        tq.GetChild(1).Call("_Ready");
        GetTree().ChangeScene("res://Battle.tscn");
    }

}
