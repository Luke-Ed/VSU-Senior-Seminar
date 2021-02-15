using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public class Player : KinematicBody2D {
    [Export]

    public int moveSpeed = 250;
    public CollisionShape2D hitbox;
    public Sprite playerSpriteNode;

    // For all the methods pertaining to stats, nothing is set in stone
    // numbers are expected to change as at a later date.
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage;
    public String CharacterClass;
    public AnimationPlayer animate;



    // Can create two different types of players one with melee stats and the other with ranged.
    // Will be able choose class at the start of the game at a main menu once implemented.
    public Player(String Class) {
        CharacterClass = Class;

        if (Class == "Melee") {
            Strength = 10;
            Dexterity = 5;
            Vitality = 10;
            Intelligence = 5;
            Luck = 5;
            AttackDamage = 5 + Strength;
        }
        else if (Class == "Ranged") {
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

    public Player() {

    }

    //When character levels up choose a stat to increase;
    public void LevelUp(String Stat) {
        Level++;
        if (Stat == "Strength") {
            Strength++;
            if (CharacterClass == "Melee") {
                AttackDamage = 5 + Strength;
            }
        }
        else if (Stat == "Dexterity") {
            Dexterity++;
            if (CharacterClass == "Ranged") {
                AttackDamage = 5 + Dexterity;
            }
        }
        else if (Stat == "Vitality") {
            Vitality++;
            MaxHealth = 5 + Vitality;
        }
        else if (Stat == "Intelligence") {
            Intelligence++;
        }
        else if (Stat == "Luck") {
            Luck++;
        }
        CurrentHealth = MaxHealth;
    }

    //Returns the amount of damage done if you are able to hit
    public int AttackEnemy(KinematicBody2D Enemy) {
        Random random = new Random();
        int roll = random.Next(101);
        //Checking to make sure the ghost can hit the enemy based on intelligence
        if(roll >= 20 - Intelligence) {
            //Checking for critical hit based on luck
            if (roll >= 100 - Luck) {
                return AttackDamage * 2;
            }
            return AttackDamage;
        }
        else {
            return 0;
        }
    }

    public void TakeDamage(int damage) {
        Random random = new Random();
        int roll = random.Next(101);
        if (roll >= 100 - Intelligence) {
            CurrentHealth -= damage;
        }
    }



    public override void _PhysicsProcess(float delta) {
        var motion = new Vector2();
        //Player will use WASD to move their character
        
        motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        /*
        if (!motion.x.Equals(0) || !motion.y.Equals(0)) {
          ChangeState("walk");
          MoveAndCollide(motion.Normalized() * moveSpeed * delta);
        }
        else {
          ChangeState("ready");
        }
        //ChangeState("ready");
        */
        var collision = MoveAndCollide(motion.Normalized() * delta * moveSpeed);
        
        if (collision != null) {
            if (collision.Collider.HasMethod("Hit")) {
                //ChangeState("dead");
                collision.Collider.Call("Hit");
            }
        }     

        // Adding animation code from Elijah's branch
        animate = (AnimationPlayer) GetNode("AnimationPlayer");
        playerSpriteNode = (Sprite) GetNode("Sprite");

        //Possession listener
        if (Input.IsActionPressed("possession")) { //If Alt is pressed
            Possess();
        }
    }
    
    public void Possess() {
        //Check if anyone within range
        Area2D possessionArea = (Area2D) GetNode("Area2D");
        Godot.Collections.Array nearby = possessionArea.GetOverlappingBodies(); //Check who is nearby
        GD.Print("count:", nearby.Count); //For debugging
        if (nearby.Count > 1) { //The player counts as one

            float closestDistance = 1000;
            int closestEnemyIndex = 0;
            Boolean enemyFound = false;

            //2. Find closest enemy
            for (int x = 0; x < nearby.Count; x++) {
                PhysicsBody2D currentEnemy = (PhysicsBody2D) nearby[x]; //Grab one
                if (currentEnemy.Name != "Player" && currentEnemy.GetGroups().Contains("Enemies")) { //Prevent possessing yourself OR an inanimate object
                    float currentDistance = currentEnemy.GlobalPosition.DistanceTo(this.GlobalPosition); //Calculate distance
                    if (currentDistance < closestDistance) { //Check if closer than current closest
                        closestEnemyIndex = x;
                        closestDistance = currentDistance;
                        enemyFound = true;
                        //GD.Print("Closest now: ", ((PhysicsBody2D)nearby[x]).Name); //For debugging
                    }
                }
            }  

            //3. Possess if enemy found
            if (enemyFound) {
                //Change state to possession state
                ChangeState("possession");
                //Grab victim
                KinematicBody2D victim = (KinematicBody2D) nearby[closestEnemyIndex];
                //Grab victim sprite
                Sprite victimSprite = (Sprite) victim.GetNode("Sprite");
                //Set player sprite to that of victim
                if (victimSprite != null && victim != null && playerSpriteNode != null) { //Prevent NullPointers
                    GD.Print("Inside");
                    GetNode(Sprite).Texture = victimSprite.Texture; //Stopped here
//                    playerSpriteNode.Texture = victimSprite.Texture;    
                }
            }
        }
    }

    // Imported from Elijah's branch, and matched names, and styles
    public void ChangeState(string newState) {
      switch (newState) {
        case "ready": {
          animate.Play("Idle");
          break;
        }
        case "dead": {
          animate.Play("Die");
          break;
        }
        case "walk": {
          animate.Play("Walking");
          break;
        }
        case "possession": {
            break;
        }
        default: {
            break;
        }
        }
    }
}