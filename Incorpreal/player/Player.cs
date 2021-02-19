using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;

    //For all the methods pertaining to stats, nothing is set in stone
    //numbers are expected to change as at a later date.

    //Can create two different types of players one with melee stats and the other with ranged.
    //Will be able choose class at the start of the game at a main menu once implemented.
        public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage;
    public String CharacterClass;
    public Player(String Class)
    {
        
        CharacterClass = Class;
        if (Class == "Melee")
        {
            Strength = 10;
            Dexterity = 5;
            Vitality = 10;
            Intelligence = 5;
            Luck = 5;
            AttackDamage = 5 + Strength;
        }
        else if (Class == "Ranged")
        {
            Strength = 5;
            Dexterity = 10;
            Vitality = 5;
            Intelligence = 10;
            Luck = 5;
            AttackDamage = 5 + Dexterity;
        }
        Experience = 0;
        MaxHealth = 5 + Vitality;
        CurrentHealth = MaxHealth;
        Level = 1;
    }

    public Player()
    {

    }


    public override void _Ready()
    {
        ResourceLoader.Load("res://Game.tscn");
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        if (gp.playerCharacter == null)
        {
            gp.createPlayer();
        }
        if (gp.playerLocation != null && gp.enemyPath != null)
        {
            GlobalPosition = gp.playerLocation;
            GetParent().GetNode(gp.enemyPath).QueueFree();
        }
        var healthLabel = GetParent().GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

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
                GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
                gp.playerLocation = GlobalPosition;
                collision.Collider.Call("Hit");
            }
        }        
    }


}
