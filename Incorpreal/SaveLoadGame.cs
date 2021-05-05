using System;
using Godot;

namespace Incorpreal {
  public class SaveLoadGame : Node{
    private GlobalPlayer _globalPlayer;
    private Player _player;
    public Godot.Collections.Dictionary<string, object> NodeData;
    public override void _Ready(){ 
      _globalPlayer = GetNode<GlobalPlayer>("/root/GlobalData");
      _player = _globalPlayer.PlayerCharacter;
    }

    public Boolean Save(Godot.Collections.Array saveables) {
      var saveFile = new File();
      saveFile.Open("user://savegame.save", File.ModeFlags.Write); //Open file in write mode

      foreach (Node saveable in saveables) { 
        //Iterate them
        if (saveable.Filename.Empty()) { 
          //Skip empty nodes
          GD.Print(String.Format("node '{0}' is not an instanced scene ", saveable.Name));
          continue;
        }
        if (!saveable.HasMethod("Save")) { 
          //Skip ones without save methods
          GD.Print("node '{0}' has no Save method", saveable.Name);
          continue;
        }
        var saveData = saveable.Call("Save");
        saveFile.StoreLine(JSON.Print(saveData));
      }

      saveFile.Close();
      return true;
    }

    public Action Unload(File saveFile, Godot.Collections.Dictionary<string, object> nodeData) {
      NodeData = nodeData;

      //Check which level its in
      string level = (string) nodeData["currentLevel"];

      //Clear old global values
      _globalPlayer.EnemiesFought.Clear();
      _globalPlayer.Inventory.Clear();
      _globalPlayer.EquippedWeapon = null;
      _globalPlayer.EquippedArmor = null;

      //Load global values
      string equippedArmor = (string)nodeData["equipedArmor"];
      if (equippedArmor != "") {
        string[] equippedArmorData = equippedArmor.Split(",");
        Item equippedArmorItem = new Item();
        equippedArmorItem.GiveProperties(equippedArmorData[0], equippedArmorData[1], equippedArmorData[2], Int16.Parse(equippedArmorData[3]));
        equippedArmorItem.SpritePath = equippedArmorData[4];
        _globalPlayer.EquippedArmor = equippedArmorItem;
      }
      string equippedWeapon = (string)nodeData["equipedWeapon"];
      if (equippedWeapon != "") {
        string[] equippedWeaponData = equippedWeapon.Split(",");
        Item equippedWeaponItem = new Item();
        equippedWeaponItem.GiveProperties(equippedWeaponData[0], equippedWeaponData[1], equippedWeaponData[2], Int16.Parse(equippedWeaponData[3]));
        equippedWeaponItem.SpritePath = equippedWeaponData[4];
        _globalPlayer.EquippedWeapon = equippedWeaponItem;
      }
      string inventory = (string)nodeData["inventory"];
      if (inventory != "") {
        string[] inventoryItems = inventory.Split("|");
        foreach(string item in inventoryItems) {
          string[] itemData = item.Split(",");
          Item newItem = new Item();
          newItem.GiveProperties(itemData[0], itemData[1], itemData[2], Int16.Parse(itemData[3]));
          newItem.SpritePath = itemData[4];
          _globalPlayer.Inventory.Add(newItem);
        }
      }
      _globalPlayer.BaseStat = (int)((float)nodeData["baseStat"]);
      Vector2 newPosition = new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]);
      _globalPlayer.PlayerLocation = newPosition;

      //Load the level
      GetTree().ChangeScene(level);

      saveFile.Close();
      System.Action loadAction = new System.Action(Delay); //Return Load() as the next action to be performed once old nodes are freed
      return loadAction;
    }

    public void Delay() {
      GD.Print("");
    }

    public void Load() {
      //Grab the player node
      Player player = null;
      foreach (Node node in GetTree().CurrentScene.GetChildren()) {
        if (node.Name == "Player") {
          player = (Player)node;
        }
      }

      //Check if the player is possessing someone or not
      if (NodeData["resPath"] != null) {
        player.resPath = (string)NodeData["resPath"];
        Texture newTexture = (Texture)ResourceLoader.Load((string)NodeData["playerSpriteNode.Texture.ResourcePath"]);
        player.playerSpriteNode.Texture = newTexture;
        player.PossessedEnemyId = (string)NodeData["PossessedEnemyId"];
        _globalPlayer.isPossesing = true;
        _globalPlayer.EnemyPossessed = (string)NodeData["EnemyPossessed"];
        if (_globalPlayer.isPossesing && _globalPlayer.EnemyPossessed != null && GetTree().CurrentScene.HasNode("/root/Node2D/Enemies/" + _globalPlayer.EnemyPossessed)) { //Remove the possessed enemy
          GetNode("/root/Node2D/Enemies/" + (string)NodeData["enemyPossessed"]).QueueFree();
        }
      } else {
        _globalPlayer.isPossesing = false;
      }

      //Reload all the necessary values
      player.moveSpeed = (int)((float)NodeData["moveSpeed"]);
      player.stuck = (Boolean)NodeData["stuck"];
      player.ExperienceToNextLevel = (int)((float)NodeData["ExperienceToNextLevel"]);
      player.AttackDamage = (int)((float)NodeData["AttackDamage"]);
      player.Level = (int)((float)NodeData["Level"]);
      player.CurrentHealth = (int)((float)NodeData["CurrentHealth"]);
      player.MaxHealth = (int)((float)NodeData["MaxHealth"]);
      player.Experience = (int)((float)NodeData["Experience"]);
      player.Luck = (int)((float)NodeData["Luck"]);
      player.Intelligence = (int)((float)NodeData["Intelligence"]);
      player.Vitality = (int)((float)NodeData["Vitality"]);
      player.Dexterity = (int)((float)NodeData["Dexterity"]);
      player.Strength = (int)((float)NodeData["Strength"]);
      Vector2 newPosition = new Vector2((float)NodeData["PosX"], (float)NodeData["PosY"]);
      player.Position = newPosition;
      player.playerSpriteNode.FlipH = (Boolean)NodeData["facingLeft"];
      string[] enemiesFought = ((string)NodeData["enemyFought"]).Split(",");
      foreach (string enemy in enemiesFought) { //Remove all enemies already defeated in combat
        if (GetTree().CurrentScene.HasNode("/root/Node2D/Enemies/" + enemy)) {
          _globalPlayer.EnemiesFought.Add(enemy);
          GetNode("/root/Node2D/Enemies/" + enemy).QueueFree(); //Level's root node must be named "Node2D" for these to work. Also enemies  must have different names (bat, bat2, etc)
        }
      }
      //Label healthLabel = (Label)GetNode("Player/Camera2D/CanvasLayer/HealthLabel");
      //healthLabel.Text = (string)nodeData["hplabel"];
      this.NodeData = null;
    }
  }
}
