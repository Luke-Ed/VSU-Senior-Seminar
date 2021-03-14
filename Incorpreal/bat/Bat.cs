using Godot;
using System;

public class Bat : KinematicBody2D
{
    [Export]

    public int moveSpeed = 150;
    public int attack;
    public int health;
    public int currentHealth;
    public string enemyName;
    public GlobalPlayer gp;
    public KinematicBody2D player;

    public override void _PhysicsProcess(float delta)
    {
        if (player != null)
        {
            Vector2 velocity = GlobalPosition.DirectionTo(player.GlobalPosition);
            MoveAndCollide(velocity * moveSpeed * delta);
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
        if (!gp.isDefending)
        {
            return gp.takeDamage(attack);
        }
        else
        {
            if (gp.didBlock)
            {
                return gp.takeDamage(0);
            }
            else
            {
                return gp.takeDamage(attack / 2);
            }
        }
    }


    public void Hit()
    {
        Sprite playerSprite = (Sprite)GetNode("../Player/Sprite/player"); //Grab player sprite
        if (playerSprite.Texture.ResourcePath.Equals("res://assets/player.png")) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
            NodePath np = GetPath();
            gp.nodePaths.Add(np);
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
            player = (KinematicBody2D)body;
        }
    }

    public void _on_Area2D_body_exited(Node body)
    {
        if (body.Name == "Player")
        {
            player = null;
        }
    }

}
