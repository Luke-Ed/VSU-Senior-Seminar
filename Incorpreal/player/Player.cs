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
    public Timer safetyTimer;
    public AnimatedSprite playerAnimatedNode;
    protected Vector2 lastDirection;
    protected String animationToPlay;

    //For all the methods pertaining to stats, nothing is set in stone
    //numbers are expected to change as at a later date.

    //Can create two different types of players one with melee stats and the other with ranged.
    //Will be able choose class at the start of the game at a main menu once implemented.
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage, ExperienceToNextLevel;
    public Player()
    {
        Strength = 5;
        Dexterity = 5;
        Vitality = 10;
        Intelligence = 5;
        Luck = 5;
        AttackDamage = 5 + Strength;
        Experience = 0;
        MaxHealth = 5 + Vitality;
        CurrentHealth = MaxHealth;
        Level = 1;
        ExperienceToNextLevel = 10;
    }

    public override void _Ready()
    {
        this.Filename = "Player";
        this.AddChild(footsteps);
        safetyTimer = new Timer();
        this.AddChild(safetyTimer, false); //Safety timer prevents player from being instantly attacked when exiting possession for the default timer duration
        safetyTimer.Connect("timeout", this, "_on_possession_timer_timeout");
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
            playerSpriteNode.Visible = false;
            playerAnimatedNode = (AnimatedSprite)GetNode("Sprite/AnimPlayer");
            playerAnimatedNode.Visible = true;
            hitbox = (Area2D)GetNode("findEmptyPosArea2D");
            possessionArea = (Area2D)GetNode("Area2D");
            stuck = false;
            Label hpLabel = (Label)GetNode("Camera2D/HealthLabel");
            gp.hplabel = hpLabel;
            gp.updateHealthLabel(gp.hplabel);
            lastDirection = new Vector2(0,1); //Keep track of player's last facing direction
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

            AnimatesPlayer(motion);
        }        
    }

    //Function to control Player's animated sprite
    public void AnimatesPlayer(Vector2 direction) 
    {
        if(direction != Vector2.Zero)
        {
            //Update last direction
            lastDirection = direction;

            //Determine which walking animation to play
            animationToPlay = GetAnimationDirection(lastDirection) + "_Walk";

            playerAnimatedNode.Frames.SetAnimationSpeed(animationToPlay, 2 + 4 * direction.Length());

            playerAnimatedNode.Play(animationToPlay);
        }

        else
        {
            //Determine which idle animation to play
            animationToPlay = GetAnimationDirection(lastDirection) + "_Idle";
            playerAnimatedNode.Play(animationToPlay);
        }
    }

    public String GetAnimationDirection(Vector2 direction)
    {
        var normDirection = direction.Normalized();
        
        if(normDirection.y >= 0.707)
        {
            return "Down";
        }

        else if(normDirection.y <= -0.707)
        {
            return "Up";
        }

        else if(normDirection.x <= -0.707)
        {
            return "Left";
        }

        else if(normDirection.x >= 0.707)
        {
            return "Right";
        }

        else
        {
            return "Down";
        }
    }
	
    //Possession listener
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("possession")) { //If R is pressed
            Possess();
        }
        //Pressing M will add this test weapon to your inventory and it should show. This will be removed just giving me the ability to add in random items for testing purposes and example.
        else if (Input.IsActionJustPressed("createItem") && Visible)
        {
            //Creating a scene of the item node.
            Node inventory = GetParent().GetNode("InventoryMenu").GetNode("Inventory");
            PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
            Item item = (Item)ItemScene.Instance();
            //Wanted two different items just so I could test to make sure things were being changed so randomly deciding bewteen the two.
            Random random = new Random();
            int roll = random.Next(10);
            if (roll % 2 == 1)
            {
                item.giveProperties("Sword", "Weapon", "Strength", 10);
                //item currently holds base gem picture can be changed like the following with a sword asset that I found online.
                item.changePicture("res://assets/sword.png");
            }
            else
            {
                item.giveProperties("Bow", "Weapon", "Dexterity", 10);
                item.changePicture("res://assets/Bow.png");
            }
            //Putting the item into list in the global player to allow the ability to keep track of them throughout scene changes.
            gp._inventory.Add(item);
            //Putting the item into an inventory slot.
            inventory.Call("fillSlot", item);
        }
        //Pressing N will reduce your health by 5 and put a health potion in player's inventory that when used will increase player's current health by 10.
        //again this is for demonstration/testing purposes only.
        else if (Input.IsActionJustPressed("createPotion") && Visible)
        {
            gp.CurrentHealth -= 5;
            gp.updateHealthLabel(gp.hplabel);
            Node inventory = GetParent().GetNode("InventoryMenu").GetNode("Inventory");
            PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
            Item item = (Item)ItemScene.Instance();
            item.giveProperties("Health Potion", "Consumable", "Health", 10);
            item.changePicture("res://assets/HealthPotion.png");
            gp._inventory.Add(item);
            inventory.Call("fillSlot", item);
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
        if (enemyFound && PossesseeName == null) {
            gp.enemyPossessed = ((Node)nearby[closestEnemyIndex]).Name;
            safetyTimer.Stop(); //Stops signal from being sent at undesireable time when repeatedly possessing
            possessee = (KinematicBody2D) nearby[closestEnemyIndex]; //Grab victim
            this.resPath = possessee.Filename; //Grab victim resource path for later
            Sprite victimSprite = (Sprite)possessee.GetNode("Sprite"); //Grab victim sprite
            this.SetCollisionMaskBit(2, true); //Make GhostWalls impenetrable while possessing
            if (resPath.Contains("Bat")) {
                this.SetCollisionLayerBit(0, false); //If possessing a bat, gain ability to fly over LowWalls. This was the only way it worked...
                this.SetCollisionMaskBit(3, false); //Turn off LowWall collisions
            }
            playerAnimatedNode.Visible = false;
            playerSpriteNode.Visible = true;
            playerSpriteNode.Texture = victimSprite.Texture; //Copy victim's texture
            PossesseeName = victimSprite.GetParent().Name;
            victimSprite.GetParent().QueueFree(); //Make enemy disappear
            gp.isPossesing = true;
        } else if (PossesseeName != null) { //Else if already possessing, undo it
            playerSpriteNode.Texture = (Texture) ResourceLoader.Load("res://assets/PlayerSpriteSingleTest.png"); //Return player sprite to normal
            playerSpriteNode.Visible = false;
            playerAnimatedNode.Visible = true;
            this.SetCollisionMaskBit(2, false); //Make GhostWalls penetrable again
            if (resPath.Contains("Bat")) { //Return from Bat mode
                this.SetCollisionLayerBit(0, true);
                this.SetCollisionMaskBit(3, true);
            }
            Vector2 newLocation = this.GlobalPosition;
            newLocation.x += 80;
            this.map.SpawnEnemy(this.resPath, newLocation, GetTree().CurrentScene, PossesseeName); //Bring original enemy back
            safetyTimer.Start();
            possessee = null;
            PossesseeName = null;
        }
    } 

    public void _on_possession_timer_timeout() {
        if (possessee == null) { //Prevents issue if quickly reentering possession after exiting
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

    public void _on_Camera_body_entered(Node body)
    {
        if (body.IsInGroup("Enemies"))
        {
            body.Set("_onCamera", true);
        }
    }

    public void _on_Camera_body_exited(Node body)
    {
        if (body.IsInGroup("Enemies"))
        {
            body.Set("_onCamera", false);
        }
    }
  
    public Godot.Collections.Dictionary<string, object> Save() {
        string spriteFileName = playerSpriteNode.Texture.ResourcePath;
        string enemiesFought = "";
        foreach (string enemyFought in gp.enemyFought) {
            enemiesFought += enemyFought + ",";
        }
        return new Godot.Collections.Dictionary<string, object>() {
            { "currentLevel", GetTree().CurrentScene.Filename },
            { "playerPath", ((string)this.GetPath()).Substring(6) },
            { "moveSpeed", moveSpeed },
            { "playerSpriteNode.Texture.ResourcePath", spriteFileName },
            { "resPath", resPath },
            { "stuck", stuck },
            { "PossesseeName", PossesseeName },
            { "currentPoints", gp.currentPoints },
            { "spiritPoints", gp.spiritPoints },
            { "baseStat", gp.baseStat },
            { "ExperienceToNextLevel", gp.ExperienceToNextLevel },
            { "AttackDamage", gp.AttackDamage },
            { "Level", gp.Level },
            { "CurrentHealth", gp.CurrentHealth },
            { "MaxHealth", gp.MaxHealth },
            { "Experience", gp.Experience },
            { "Luck", gp.Luck },
            { "Intelligence", gp.Intelligence },
            { "Vitality", gp.Vitality },
            { "Dexterity", gp.Dexterity },
            { "Strength", gp.Strength },
            { "isPossesing", gp.isPossesing },
            { "PosX", GlobalPosition.x }, // Vector2 is not supported by JSON
            { "PosY", GlobalPosition.y },
            { "enemyFought", enemiesFought },
            { "hplabel", gp.hplabel.Text },
			{ "facingLeft", playerSpriteNode.FlipH },
            { "enemyPossessed", gp.enemyPossessed }
        };
    }
}