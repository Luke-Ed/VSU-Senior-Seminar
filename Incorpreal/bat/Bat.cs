using Godot;
using System;
using System.Collections.Generic;

public class Bat : KinematicBody2D
{
    [Export]
    public int moveSpeed = 50;
    public int attack;
    public int health;
    public int currentHealth;
    public string enemyName;
    public GlobalPlayer gp;
    public KinematicBody2D player;
    //private Navigation2D _navigation;

    public override void _PhysicsProcess(float delta)
    {
        try
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            MoveAndCollide(direction * moveSpeed * delta);
        }
        catch
        {

        }
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
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        Boolean didHit = false;
        if (!gp.isDefending)
        {
            didHit = gp.takeDamage(attack);
        }
        else
        {
            if (gp.didBlock)
            {
                didHit = gp.takeDamage(0);
            }
            else
            {
                didHit = gp.takeDamage(attack / 2);
            }
        }
        if (didHit)
        {
            switch (enemyName)
            {
                case ("Bat"):
                    if (tq.enemyCurrentHP + attack <= tq.enemyMaxHP)
                    {
                        tq.enemyCurrentHP += attack;
                    }
                    break;
                case ("Bear"):
                    gp.status = "Bleeding";
                    break;
                case ("Goblin"):
                    //Need something for goblin
                    break;
                case ("Skeleton"):
                    //Need something for skeleton
                    break;
                case ("Snake"):
                    //Need something for snake
                    break;
                case ("Wolf"):
                    //Need something for wolf
                    break;
                default:
                    break;
            }
        }
        return didHit;
    }


    public void Hit()
    { 
        if (!gp.isPossesing) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
            gp.enemyFought.Add(this.Name);
            TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
            tq.GetChild(1).Name = enemyName;
            tq.GetChild(1).Call("_Ready");
            GetTree().ChangeScene("res://Battle.tscn");
        }
    }

    public void _on_Area2D_body_entered(Node body)
    {
        if (body.Name == "Player")
        {
            Console.WriteLine("Entered");
            player = (KinematicBody2D)body;
        }
    }

    public void _on_Area2D_body_exited(Node body)
    {
        if (body.Name == "Player")
        {
            Console.WriteLine("left");
            player = null;
        }
    }
}
