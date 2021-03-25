using Godot;
using System;

public class Player : KinematicBody2D {
  [Export] public int moveSpeed = 125;
  public PhysicsBody2D possessee;	
  public string resPath;	
  public Map map = new Map();
  public Area2D hitbox;
  public Sprite playerSpriteNode;
  public AnimationPlayer animate;
  public Area2D possessionArea;
  public Boolean stuck;
  public GlobalPlayer gp;
  public String PossesseeName;

  //For all the methods pertaining to stats, nothing is set in stone
  //numbers are expected to change as at a later date.

  //Can create two different types of players one with melee stats and the other with ranged.
  //Will be able choose class at the start of the game at a main menu once implemented.
    
  // L: I moved stats into properties, which allows them to be safely updated from other classes, and makes working 
  // with them in other classes a little easier.
  public int Strength { get; set; }
  public int Dexterity { get; set; }
  public int Vitality { get; set; }
  public int Intelligence { get; set; }
  public int Luck { get; set; }
  public int Experience { get; set; }
  public int MaxHealth { get; set; }
  public int CurrentHealth { get; set; }
  public int Level { get; set; }
  public int AttackDamage { get; set; }
  public int ExperienceToNextLevel { get; set; }
  public String CharacterPlayerClass { get; }
    
  public Player(String playerClass) {
    CharacterPlayerClass = playerClass;
    if (playerClass == "Melee") {
      Strength = 10;
      Dexterity = 5;
      Vitality = 10;
      Intelligence = 5;
      Luck = 5;
      AttackDamage = 5 + Strength;
    }
    else if (playerClass == "Ranged") {
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
    ExperienceToNextLevel = 10;
  }

  public override void _Ready() {
    gp = (GlobalPlayer)GetNode("/root/GlobalData");
    //Eventually a main menu will already have a character made for the player
    //This is for demonstration purposes
    if (gp.PlayerCharacter == null) {
      gp.createPlayer();
    }
    if (gp.enemiesFought.Count > 0) {
      GlobalPosition = gp.PlayerLocation;
      for (int i = 0; i < gp.enemiesFought.Count; i++) {
        Console.WriteLine(gp.enemiesFought[i]);
        GetParent().FindNode(gp.enemiesFought[i]).QueueFree();
      }
    }
    animate = GetNode<AnimationPlayer>("AnimationPlayer") as AnimationPlayer;
    playerSpriteNode = (Sprite)GetNode("Sprite/player");
    hitbox = (Area2D)GetNode("findEmptyPosArea2D");
    possessionArea = (Area2D)GetNode("Area2D");
    stuck = false;
  }

  public Boolean AttackEnemy() {
    return gp.AttackEnemy();
  }

  public void CastSpell() {
    gp.castSpell();
  }

    public override void _PhysicsProcess(float delta)
    {
        if (Visible)
        {
            var motion = new Vector2();
            //Player will use WASD to move their character
            motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

            MoveAndCollide(motion.Normalized() * moveSpeed * delta);

            if (!motion.x.Equals(0) || !motion.y.Equals(0))
            {
                stuck = false;
                ChangeState("Walking");
                if (motion.x > 0)
                { //If walking right
                    playerSpriteNode.FlipH = false; //Character faces right
                }
                else if (motion.x < 0)
                { //If walking left
                    playerSpriteNode.FlipH = true; //Character faces left
                }
            }
            else
            {
                ChangeState("Idle");
            }


            var collision = MoveAndCollide(motion.Normalized() * delta * moveSpeed);

            if (collision != null)
            {
                if (collision.Collider.HasMethod("Hit"))
                {
                    gp = (GlobalPlayer)GetNode("/root/GlobalData");
                    gp.lastScene = GetTree().CurrentScene.Filename;
                    gp.PlayerLocation = GlobalPosition;
                    collision.Collider.Call("Hit");
                }
                else if (!movementPossible())
                {
                    stuck = true;
                    teleport();
                }
            }
        }        
    }
	
    //Possession listener
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("possession")) { //If R is pressed
            Possess();
        }
    }
	
    public void ChangeState(string newState)
    {
        switch (newState) {
	      case "Idle": {
		      animate.Play("Idle");
		      break;
	      }
	      case "Dead": {
	        animate.Play("Die");
	        break;
	      }
	      case "Walking": {
          animate.Play("Walking");
	        break;
	      }
	      default: {
	        break;
	      }
	  }
}

    public void Possess() {
        //1. Check if anyone within range
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
            playerSpriteNode.Texture = victimSprite.Texture; //Copy victim's texture
            PossesseeName = victimSprite.GetParent().Name;
            victimSprite.GetParent().QueueFree(); //Make enemy disappear
            gp.isPossesing = true;
        } else if (possessee != null) { //Else if already possessing, undo it
            playerSpriteNode.Texture = (Texture) ResourceLoader.Load("res://assets/player.png"); //Return player sprite to normal
            this.SetCollisionMaskBit(2, false); //Make GhostWalls penetrable again
            if (resPath.Contains("Bat")) { //Return from Bat mode
                this.SetCollisionLayerBit(0, true);
                this.SetCollisionMaskBit(3, true);
            }
            Vector2 newLocation = this.GlobalPosition;
            newLocation.x += 70;
            this.map.SpawnEnemy(this.resPath, newLocation, GetTree().CurrentScene, PossesseeName); //Bring original enemy back
            possessee = null;
            gp.isPossesing = false;
        }
    } 

    //This method returns a Boolean denoting if player movement is possible in any direction
    public Boolean movementPossible() {
        Boolean movementPossible = true;
        if (TestMove(this.Transform, new Vector2(1,0)) && TestMove(this.Transform, new Vector2(-1,0)) && TestMove(this.Transform, new Vector2(0,-1)) && TestMove(this.Transform, new Vector2(0,1))) { //Test all 4 directions
            movementPossible = false;
        }
        return movementPossible;
    }

    //This method allows the player to teleport if they get stuck in a wall or between impassible objects, by clicking where you want to go
    public void teleport() {
        if (Input.IsActionJustReleased("left_click")) {
            Vector2 mousePos = GetViewport().GetMousePosition();
            this.Position = mousePos;
            stuck = false;
        }
    }
}
