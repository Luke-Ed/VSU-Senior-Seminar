using Godot;
using System;

public class Player : KinematicBody2D {
    [Export]

    public int moveSpeed = 125;
    public PhysicsBody2D possessee = null;
    public string resPath;
    public Map map = new Map();
    public CollisionShape2D hitbox;
    public Sprite playerSpriteNode;
    public AnimationPlayer animate;


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
        var healthLabel = GetParent().GetNode<Label>("HealthLabel") as Label; //This line is giving us the "HealthLabel not found" error
        gp.updateHealthLabel(healthLabel);
    }

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();
        //Player will use WASD to move their character
        //animate = (AnimationPlayer) GetNode("AnimationPlayer");
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

         MoveAndCollide(motion.Normalized() * moveSpeed * delta);

        var collision = MoveAndCollide(motion.Normalized() * delta);


        if (collision != null)
        {
            if (collision.Collider.HasMethod("Hit"))
            {
                GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
                gp.playerLocation = GlobalPosition;
                //ChangeState("dead");
                collision.Collider.Call("Hit");
            }
        } 

        //Possession listener
        if (Input.IsActionJustPressed("possession")) { //If R is pressed
            Possess();
        }       
    }
	
	// Imported from Elijah's branch, and matched names, and styles
    /*
	public void ChangeState(string newState) {
	  switch (newState) {
		case "ready": {
		  animate.Play("Idle");
		  break;
		}
		case "dead": {
		  animate.Stop();
		  animate.Play("Die");
		  break;
		}
		case "walk": {
		  animate.Stop();
		  animate.Play("Walking");
		  break;
		}
		default: {
		  break;
		}
	  }
	}
    */

     public void Possess() {
        //1. Check if anyone within range
        Area2D possessionArea = (Area2D)GetNode("Area2D");
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
            
            //3. If suitable enemy found & player not already possessing someone, possess that enemy
            if (enemyFound && this.possessee == null) {
                possessee = (KinematicBody2D) nearby[closestEnemyIndex]; //Grab victim
                this.resPath = possessee.Filename; //Grab victim resource path for later
                Sprite victimSprite = (Sprite) possessee.GetNode("Sprite"); //Grab victim sprite
                this.SetCollisionMaskBit(2, true); //Make GhostWalls impenetrable while possessing
                if (resPath.Contains("Bat")) {
                    this.SetCollisionLayerBit(0, false); //If possessing a bat, gain ability to fly over LowWalls. This was the only way it worked...
                    this.SetCollisionMaskBit(3, false); //Turn off LowWall collisions
                }
                //Possession animation here (optional)
                this.playerSpriteNode = (Sprite) GetNode("Sprite/player");
                playerSpriteNode.Texture = victimSprite.Texture; //Copy victim's texture
                victimSprite.GetParent().QueueFree(); //Make enemy disappear
            } else if (possessee != null) { //Else if already possessing, undo it
                playerSpriteNode.Texture = (Texture) ResourceLoader.Load("res://assets/player.png"); //Return player sprite to normal
                this.SetCollisionMaskBit(2, false); //Make GhostWalls penetrable again
                if (resPath.Contains("Bat")) { //Return from Bat mode
                    this.SetCollisionLayerBit(0, true);
                    this.SetCollisionMaskBit(3, true);
                }
                this.map.SpawnEnemy(this.resPath, this.GlobalPosition, GetTree().CurrentScene); //Bring original enemy back
                possessee = null;
            }
    }
}
