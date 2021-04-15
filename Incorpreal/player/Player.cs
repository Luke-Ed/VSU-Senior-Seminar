using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D {
    [Export]
    public int moveSpeed = 100;
    public PhysicsBody2D possessee = null;	
    public string resPath;	
    public Map map = new Map();
    public Area2D hitbox;
    public Sprite playerSpriteNode;
    public AnimationPlayer animate;
    public AudioStreamPlayer2D footsteps = new AudioStreamPlayer2D();
    public Area2D possessionArea;
    public Boolean stuck;
    public GlobalPlayer gp;
    public String PossesseeName;

    //For all the methods pertaining to stats, nothing is set in stone
    //numbers are expected to change as at a later date.

    //Can create two different types of players one with melee stats and the other with ranged.
    //Will be able choose class at the start of the game at a main menu once implemented.
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage, ExperienceToNextLevel;
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
        ExperienceToNextLevel = 10;
    }

    public Player()
    {

    }

    public override void _Ready()
    {
        this.AddChild(footsteps);
        const string Path = "res://sounds/footsteps.wav";
        AudioStream footstep = (AudioStream)GD.Load(Path);
        footsteps.Stream = footstep;
        footsteps.VolumeDb = (0);
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        //Eventually a main menu will already have a character made for the player
        //This is for demonstration purposes
        if (gp.playerCharacter == null)
        {
            gp.createPlayer();
        }
        if (gp.playerLocation != null && gp.enemyFought.Count > 0)
        {
            GlobalPosition = gp.playerLocation;
            for (int i = 0; i < gp.enemyFought.Count; i++)
            {
                GetParent().FindNode(gp.enemyFought[i]).QueueFree();
            }
        }
            animate = GetNode<AnimationPlayer>("AnimationPlayer") as AnimationPlayer;
            playerSpriteNode = (Sprite)GetNode("Sprite/player");
            hitbox = (Area2D)GetNode("findEmptyPosArea2D");
            possessionArea = (Area2D)GetNode("Area2D");
            stuck = false;
            Label hpLabel = (Label)GetNode("Camera2D").GetNode("CanvasLayer").GetNode("HealthLabel");
            gp.hplabel = hpLabel;
            gp.updateHealthLabel(gp.hplabel);
    }

    public Boolean attackEnemy()
    {
        return gp.AttackEnemy();
    }

    public void castSpell()
    {
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
                if (footsteps.Playing == false)
                {
                    footsteps.Play();
                }
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
                footsteps.Stop();
                ChangeState("Idle");
            }


            var collision = MoveAndCollide(motion.Normalized() * delta * moveSpeed);

            if (collision != null)
            {
                if (collision.Collider.HasMethod("Hit"))
                {
                    gp = (GlobalPlayer)GetNode("/root/GlobalData");
                    gp.lastScene = GetTree().CurrentScene.Filename;
                    gp.playerLocation = GlobalPosition;
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
        //Pressing M will add this test weapon to your inventory and it should show. This will be removed just giving me the ability to add in random items for testing purposes
        else if (Input.IsActionJustPressed("createItem") && Visible)
        {
            PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
            Item item = (Item)ItemScene.Instance();
            Random random = new Random();
            int roll = random.Next(10);
            if (roll % 2 == 1)
            {
                item.giveProperties("Sword", "Weapon", "Strength", 10);
            }
            else
            {
                item.giveProperties("Bow", "Weapon", "Dexterity", 10);
            }
            gp._inventory.Add(item);
            GetParent().GetNode("InventoryMenu").GetNode("Inventory").Call("fillSlot", item);
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
        for (int x = 0; x < nearby.Count; x++) 
        { //Iterate them
            try
            {
                PhysicsBody2D currentEnemy = (PhysicsBody2D)nearby[x]; //Grab one
                if (currentEnemy.GetGroups().Contains("Enemies"))
                { //Skip bodies not belonging to the Enemies group
                    float currentDistance = currentEnemy.GlobalPosition.DistanceTo(this.GlobalPosition); //Calculate distance
                    if (currentDistance < closestDistance)
                    { //Check if closer than current closest
                        closestEnemyIndex = x;
                        closestDistance = currentDistance;
                        enemyFound = true;
                    }
                }
            }
            catch
            {

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
            newLocation.x += 80;
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

    //Still under construction
    public Godot.Collections.Dictionary<string, object> Save() {
        return new Godot.Collections.Dictionary<string, object>() {
            { "moveSpeed", moveSpeed},
            { "possessee", possessee},
            { "resPath", resPath},
            { "map", map},
            { "playerSpriteNode", playerSpriteNode},
            { "stuck", stuck},
            { "gp", gp},
            { "PossesseeName", PossesseeName},
            { "ExperienceToNextLevel", ExperienceToNextLevel},
            { "AttackDamage", AttackDamage},
            { "Level", Level},
            { "CurrentHealth", CurrentHealth},
            { "MaxHealth", MaxHealth},
            { "Experience", Experience},
            { "Luck", Luck },
            { "Intelligence", Intelligence },
            { "Vitality", Vitality },
            { "Dexterity", Dexterity },
            { "Strength", Strength },
            { "CharacterClass", CharacterClass },    
            { "Filename", this.Filename },
            { "Parent", GetParent().GetPath() },
            { "PosX", Position.x }, // Vector2 is not supported by JSON
            { "PosY", Position.y },
        };
    }
}
