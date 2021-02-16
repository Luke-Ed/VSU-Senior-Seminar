using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]

    public int moveSpeed = 250;

    //For all the methods pertaining to stats, nothing is set in stone
    //numbers are expected to change as at a later date.
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage;
    public String CharacterClass;
    public Vector2 playerPosition;


    //Can create two different types of players one with melee stats and the other with ranged.
    //Will be able choose class at the start of the game at a main menu once implemented.
    public Player(String Class)
    {
        this.CharacterClass = Class;

        if (Class == "Melee")
        {
            this.Strength = 10;
            this.Dexterity = 5;
            this.Vitality = 10;
            this.Intelligence = 5;
            this.Luck = 5;
            this.AttackDamage = 5 + this.Strength;
        }
        else if (Class == "Ranged")
        {
            this.Strength = 5;
            this.Dexterity = 10;
            this.Vitality = 5;
            this.Intelligence = 10;
            this.Luck = 5;
            this.AttackDamage = 5 + Dexterity;
        }
        this.Experience = 0;
        this.MaxHealth = 5 + Vitality;
        this.CurrentHealth = MaxHealth;
        this.Level = 1;
    }

    public Player()
    {

    }

    //When character levels up choose a stat to increase;
    public void LevelUp(String Stat)
    {
        Level++;
        if (Stat == "Strength")
        {
            Strength++;
            if (CharacterClass == "Melee")
            {
                AttackDamage = 5 + Strength;
            }
        }
        else if (Stat == "Dexterity")
        {
            Dexterity++;
            if (CharacterClass == "Ranged")
            {
                AttackDamage = 5 + Dexterity;
            }
        }
        else if (Stat == "Vitality")
        {
            Vitality++;
            MaxHealth = 5 + Vitality;
        }
        else if (Stat == "Intelligence")
        {
            Intelligence++;
        }
        else if (Stat == "Luck")
        {
            Luck++;
        }
        CurrentHealth = MaxHealth;
    }

    //Returns the amount of damage done if you are able to hit
    public int AttackEnemy(KinematicBody2D Enemy)
    {
        Random random = new Random();
        int roll = random.Next(101);
        //Checking to make sure the ghost can hit the enemy based on intelligence
        if(roll >= 20 - Intelligence)
        {
            //Checking for critical hit based on luck
            if (roll >= 100 - Luck)
            {
                return AttackDamage * 2;
            }
            return AttackDamage;
        }
        else
        {
            return 0;
        }
    }

    public void takeDamage(int damage)
    {
        Random random = new Random();
        int roll = random.Next(101);
        if (roll >= 100 - Intelligence)
        {
            CurrentHealth -= damage;
        }
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
                this.playerPosition = this.GlobalPosition;

                collision.Collider.Call("Hit");
            }
        }        
    }


}
