using Godot;
using System;
using Incorpreal;

public class Player : KinematicBody2D {
  [Export]
  public int moveSpeed = 100;
  private PhysicsBody2D _possessedEnemy = null;	
  public string resPath;
  public Map map = new Map();
  public Area2D hitbox;
  public Sprite playerSpriteNode;
  public AnimationPlayer animate;
  public Area2D possessionArea;
  public Boolean stuck;
  private GlobalPlayer _globalPlayer;
  public string PossessedEnemyId;
  public AudioStreamPlayer2D footsteps = new AudioStreamPlayer2D();
  public AnimatedSprite playerAnimatedNode;
  protected Vector2 lastDirection;
  protected String animationToPlay;
  public Timer safetyTimer;


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
  public int CurrentSpiritPoints { get; set; }
  public int MaxSpiritPoints { get; set; }
  public int ExperienceToNextLevel { get; set; }
  public String StatusEffect { get; set; }
  
  
    //Can create two different types of players one with melee stats and the other with ranged.
    //Will be able choose class at the start of the game at a main menu once implemented.
  public Player() {
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
    StatusEffect = String.Empty;
  }

    public override void _Ready() {
    // Add Footsteps sound.
    AddChild(footsteps);
    const string Path = "res://sounds/footsteps.wav";
    AudioStream footstep = (AudioStream)GD.Load(Path);
    footsteps.Stream = footstep;
    footsteps.VolumeDb = (0);
    
    //Safety timer prevents player from being instantly attacked when exiting possession for the default timer duration
    safetyTimer = new Timer();
    AddChild(safetyTimer, false);
    safetyTimer.Connect("timeout", this, "_on_possession_timer_timeout");
    
    // Load the GlobalPlayer
    _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
    //Eventually a main menu will already have a character made for the player
    //This is for demonstration purposes
    if (_globalPlayer.PlayerCharacter == null) {
      _globalPlayer.createPlayer();
    }
    if (_globalPlayer.PlayerLocation != null && _globalPlayer.EnemiesFought.Count > 0) {
      GlobalPosition = _globalPlayer.PlayerLocation;
      for (int i = 0; i < _globalPlayer.EnemiesFought.Count; i++) {
        GetParent().FindNode(_globalPlayer.EnemiesFought[i]).QueueFree();
      }
    }
    animate = GetNode<AnimationPlayer>("AnimationPlayer");
    playerSpriteNode = (Sprite)GetNode("Sprite/player");
    hitbox = (Area2D)GetNode("findEmptyPosArea2D");
    possessionArea = (Area2D)GetNode("Area2D");
    stuck = false;
    Label hpLabel = GetNode<Label>("Camera2D/HealthLabel");
    _globalPlayer.hplabel = hpLabel;
    _globalPlayer.updateHealthLabel(_globalPlayer.hplabel);
  }

  public Boolean AttackEnemy() {
    return _globalPlayer.AttackEnemy();
  }

  public void CastSpell() {
    _globalPlayer.castSpell();
  }

  public override void _PhysicsProcess(float delta){
    if (Visible){
      var motion = new Vector2();
      //Player will use WASD to move their character
      motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
      motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
      
      MoveAndCollide(motion.Normalized() * moveSpeed * delta);

      if (!motion.x.Equals(0) || !motion.y.Equals(0)){
        if (footsteps.Playing == false){
          footsteps.Play();
        }
        stuck = false;
        ChangeState("Walking");
        if (motion.x > 0){ 
          //If walking right
          playerSpriteNode.FlipH = false; 
          //Character faces right
        }
        else if (motion.x < 0) {
          //If walking left
          playerSpriteNode.FlipH = true; 
          //Character faces left
        }
      }
      else {
        footsteps.Stop();
        ChangeState("Idle");
      }
      
      var collision = MoveAndCollide(motion.Normalized() * delta * moveSpeed);
      if (collision != null) {
        if (collision.Collider.HasMethod("Hit")) {
          _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
          _globalPlayer.lastScene = GetTree().CurrentScene.Filename;
          _globalPlayer.PlayerLocation = GlobalPosition;
          collision.Collider.Call("Hit");
        }
        else if (!MovementPossible()) {
          stuck = true;
          Teleport();
        }
      }
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

  public String GetAnimationDirection(Vector2 direction){
    var normDirection = direction.Normalized();
    
    if(normDirection.y >= 0.707){
      return "Down";
    }

    else if(normDirection.y <= -0.707){
      return "Up";
    }

    else if(normDirection.x <= -0.707){
      return "Left";
    }

    else if(normDirection.x >= 0.707){
      return "Right";
    }

    else{
      return "Down";
    }
  }
  
    //Possession listener
  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed("possession")) {
      //If R is pressed
      Possess();
    }
    /*
    //Pressing M will add this test weapon to your inventory and it should show. This will be removed just giving me the ability to add in random items for testing purposes and example.
    else if (Input.IsActionJustPressed("createItem") && Visible) {
      //Creating a scene of the item node.
      Node inventory = GetParent().GetNode("InventoryMenu").GetNode("Inventory");
      PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
      Item item = (Item)ItemScene.Instance();
      //Wanted two different items just so I could test to make sure things were being changed so randomly deciding bewteen the two.
      Random random = new Random();
      int roll = random.Next(10);
      if (roll % 2 == 1) {
        item.GiveProperties("Sword", "Weapon", "Strength", 10);
        //item currently holds base gem picture can be changed like the following with a sword asset that I found online.
        item.changePicture("res://assets/sword.png");
      }
      else {
        item.GiveProperties("Bow", "Weapon", "Dexterity", 10);
        item.changePicture("res://assets/Bow.png");
      }
      //Putting the item into list in the global player to allow the ability to keep track of them throughout scene changes.
        _globalPlayer.Inventory.Add(item);
        //Putting the item into an inventory slot.
        inventory.Call("fillSlot", item);
    }
    //Pressing N will reduce your health by 5 and put a health potion in player's inventory that when used will increase player's current health by 10.
    //again this is for demonstration/testing purposes only.
    else if (Input.IsActionJustPressed("createPotion") && Visible) {
        _globalPlayer.PlayerCharacter.CurrentHealth -= 5;
        _globalPlayer.updateHealthLabel(_globalPlayer.hplabel);
        Node inventory = GetParent().GetNode("InventoryMenu").GetNode("Inventory");
        PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
        Item item = (Item)ItemScene.Instance();
        item.GiveProperties("Health Potion", "Consumable", "Health", 10);
        item.changePicture("res://assets/HealthPotion.png");
        _globalPlayer.Inventory.Add(item);
        inventory.Call("fillSlot", item);
    }
    */
  }
  
  
  private void ChangeState(string newState) {
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
        try {
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
        catch {

        }
      }

      //3. If suitable enemy found & player not already possessing someone, possess that enemy
        if (enemyFound && PossessedEnemyId == null) {
            //_globalPlayer.enemyPossessed = ((Node)nearby[closestEnemyIndex]).Name;
            safetyTimer.Stop(); //Stops signal from being sent at undesireable time when repeatedly possessing
            _possessedEnemy = (KinematicBody2D) nearby[closestEnemyIndex]; //Grab victim
            this.resPath = _possessedEnemy.Filename; //Grab victim resource path for later
            Sprite victimSprite = (Sprite)_possessedEnemy.GetNode("Sprite"); //Grab victim sprite
            this.SetCollisionMaskBit(2, true); //Make GhostWalls impenetrable while possessing
            if (resPath.Contains("Bat")) {
                this.SetCollisionLayerBit(0, false); //If possessing a bat, gain ability to fly over LowWalls. This was the only way it worked...
                this.SetCollisionMaskBit(3, false); //Turn off LowWall collisions
            }
            playerAnimatedNode.Visible = false;
            playerSpriteNode.Visible = true;
            playerSpriteNode.Texture = victimSprite.Texture; //Copy victim's texture
            PossessedEnemyId = victimSprite.GetParent().Name;
            victimSprite.GetParent().QueueFree(); //Make enemy disappear
            _globalPlayer.isPossesing = true;
        } else if (PossessedEnemyId != null) { //Else if already possessing, undo it
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
            this.map.SpawnEnemy(this.resPath, newLocation, GetTree().CurrentScene, PossessedEnemyId); //Bring original enemy back
            safetyTimer.Start();
            _possessedEnemy = null;
            PossessedEnemyId = null;
        }
    }
        

        //This method returns a Boolean denoting if player movement is possible in any direction
  public Boolean MovementPossible() {
    Boolean movementPossible = true;
    if (TestMove(Transform, new Vector2(1, 0)) && TestMove(Transform, new Vector2(-1, 0)) &&
        TestMove(Transform, new Vector2(0, -1)) &&
        TestMove(Transform, new Vector2(0, 1))) { //Test all 4 directions
      movementPossible = false; 
    }

    return movementPossible;
  }

  //This method allows the player to teleport if they get stuck in a wall or between impassible objects, by clicking where you want to go
  public void Teleport() {
    if (Input.IsActionJustReleased("left_click")) {
      Vector2 mousePos = GetViewport().GetMousePosition();
      Position = mousePos;
      stuck = false;
    }
  }
  

  public void _on_possession_timer_timeout() {
    if (PossessedEnemyId == null) { 
      //Prevents issue if quickly reentering possession after exiting
      _globalPlayer.isPossesing = false;
    }
  }
  
  

  public void _on_Camera_body_entered(Node body) {
    if (body.IsInGroup("Enemies")) {
      body.Set("_onCamera", true);
    }
  }

  public void _on_Camera_body_exited(Node body) {
    if (body.IsInGroup("Enemies")) {
      body.Set("_onCamera", false);
    }
  }

  public Godot.Collections.Dictionary<string, object> Save() {
    string spriteFileName = playerSpriteNode.Texture.ResourcePath;
    string enemiesFought = "";
    string inventory = "";
    string equipedWeapon = "";
    string equipedArmor = "";
    if (_globalPlayer.EquippedWeapon != null) {
      equipedWeapon = _globalPlayer.EquippedWeapon.Name + "," +
                      _globalPlayer.EquippedWeapon.Type + "," + 
                      _globalPlayer.EquippedWeapon.Stat + "," +
                      _globalPlayer.EquippedWeapon.Bonus + "," + 
                      _globalPlayer.EquippedWeapon.SpritePath.ToString();
    }

    if (_globalPlayer.EquippedArmor != null) {
      equipedArmor = _globalPlayer.EquippedArmor.Name + "," +
                     _globalPlayer.EquippedArmor.Type + "," + 
                     _globalPlayer.EquippedArmor.Stat + "," +
                     _globalPlayer.EquippedArmor.Bonus + "," + 
                     _globalPlayer.EquippedArmor.SpritePath.ToString();
    }

    foreach (string enemyFought in _globalPlayer.EnemiesFought) {
      //Extract enemyFought into string form (JSON doesn't play well with List<T> objects)
      enemiesFought += enemyFought + ",";
    }

    foreach (Item item in _globalPlayer.Inventory) {
      inventory += item.Name + "," + item.Type + "," + item.Stat + "," + item.Bonus + "," +
                   item.SpritePath.ToString() + "|";
    }

    return new Godot.Collections.Dictionary<string, object>() {
      {"currentLevel", GetTree().CurrentScene.Filename},
      {"playerPath", ((string) this.GetPath()).Substring(6)},
      {"moveSpeed", moveSpeed},
      {"playerSpriteNode.Texture.ResourcePath", spriteFileName},
      {"resPath", resPath},
      {"stuck", stuck},
      {"PossessedEnemyId", PossessedEnemyId},
      {"CurrentSpiritPoints", CurrentSpiritPoints},
      {"MaxSpiritPoints", MaxSpiritPoints},
      {"BaseStat", _globalPlayer.BaseStat},
      {"ExperienceToNextLevel", ExperienceToNextLevel},
      {"AttackDamage", AttackDamage},
      {"Level", Level},
      {"CurrentHealth", CurrentHealth},
      {"MaxHealth", MaxHealth},
      {"Experience", Experience},
      {"Luck", Luck},
      {"Intelligence", Intelligence},
      {"Vitality", Vitality},
      {"Dexterity", Dexterity},
      {"Strength", Strength},
      {"isPossesing", _globalPlayer.isPossesing},
      {"PosX", GlobalPosition.x}, // Vector2 is not supported by JSON
      {"PosY", GlobalPosition.y},
      {"enemyFought", enemiesFought},
      {"hplabel", _globalPlayer.hplabel.Text},
      {"facingLeft", playerSpriteNode.FlipH},
      {"EnemyPossessed", _globalPlayer.EnemyPossessed},
      {"inventory", inventory},
      {"equipedWeapon", equipedWeapon},
      {"equipedArmor", equipedArmor}
    };
  }
}

