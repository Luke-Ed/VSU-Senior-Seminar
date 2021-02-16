using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public class Player : KinematicBody2D {
    [Export]

    public int moveSpeed = 250;
    public PhysicsBody2D possessee = null;
    public string resPath;
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
        playerSpriteNode = (Sprite) GetNode("Sprite/player");

        //Possession listener
        if (Input.IsActionJustPressed("possession")) { //If R is pressed
            Possess();
        }
    }
    
    public void Possess() {
        //1. Check if anyone within range
        Area2D possessionArea = (Area2D) GetNode("Area2D");
        Godot.Collections.Array nearby = possessionArea.GetOverlappingBodies(); //Check who is nearby
            float closestDistance = 1000;
            int closestEnemyIndex = 0;
            Boolean enemyFound = false;

            //2. Find closest enemy
            for (int x = 0; x < nearby.Count; x++) { //Iterate them
                PhysicsBody2D currentEnemy = (PhysicsBody2D) nearby[x]; //Grab one
                if (currentEnemy.GetGroups().Contains("Enemies")) { //Skip bodies not belonging to the Enemies group
                    float currentDistance = currentEnemy.GlobalPosition.DistanceTo(this.GlobalPosition); //Calculate distance
                    if (currentDistance < closestDistance) { //Check if closer than current closest
                        closestEnemyIndex = x;
                        closestDistance = currentDistance;
                        enemyFound = true;
                    }
                }
            }  

            //3. Save original locations for switching back later
            Vector2 originalPlayerPos = this.GlobalPosition;
            Vector2 enemyPos = ((PhysicsBody2D)nearby[closestEnemyIndex]).GlobalPosition;

            //4. If suitable enemy found & player not already possessing someone, possess that enemy
            if (enemyFound && this.possessee == null) {
                //Grab victim
                possessee = (KinematicBody2D) nearby[closestEnemyIndex];
                //Grab victim resource path for later
                this.resPath = possessee.Filename;
                //Grab victim sprite
                Sprite victimSprite = (Sprite) possessee.GetNode("Sprite");
                //Possession animation here (optional)
                playerSpriteNode.Texture = victimSprite.Texture; //Copy victim's texture
                victimSprite.GetParent().QueueFree(); //Make enemy disappear
                this.GlobalPosition = enemyPos; //Place player in enemy's old position to complete the illusion
            } else if (possessee != null) { //Else if already possessing, undo it
                //Return player sprite to normal
                playerSpriteNode.Texture = (Texture) ResourceLoader.Load("res://assets/player.png");
                //Return player to original position
                this.GlobalPosition = originalPlayerPos;
                //Bring original enemy back
                LevelScript temp = new LevelScript();
                temp.SpawnEnemy(this.resPath, enemyPos, GetTree().CurrentScene);
                possessee = null;
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
        default: {
          break;
        }
      }
 }
}