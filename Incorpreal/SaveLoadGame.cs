using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Incorpreal;

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

    //Load the level
    GetTree().ChangeScene("res://levels/" + level + ".tscn");

    saveFile.Close();
    Action loadAction = (Load); //Return Load() as the next action to be performed once old nodes are freed
    return loadAction;
  }

  public void Load() {
    //Grab the player node
    Player player = (Player)GetTree().Root.GetNode("Level 1/Player");

    //Check if the player is possessing someone or not
    if (NodeData["resPath"] != null) {
        player.resPath = (string)NodeData["resPath"];
        player.playerSpriteNode.Texture.ResourcePath = (string)NodeData["playerSpriteNode.Texture.ResourcePath"];
        player.PossessedEnemyId = (string)NodeData["PossessedEnemyId"];
        _globalPlayer.isPossesing = true;
    } else {
      _globalPlayer.isPossesing = false;
    }

    //Reload all the necessary values
    player.moveSpeed = (int)((float)NodeData["moveSpeed"]);
    player.stuck = (Boolean)NodeData["stuck"];
    _player.CurrentSpiritPoints = (int)((float)NodeData["currentPoints"]);
    _player.CurrentSpiritPoints = (int)((float)NodeData["spiritPoints"]);
    _globalPlayer.BaseStat = (int)((float)NodeData["baseStat"]);
    _player.ExperienceToNextLevel = (int)((float)NodeData["ExperienceToNextLevel"]);
    player.ExperienceToNextLevel = player.ExperienceToNextLevel;
    _player.AttackDamage = (int)((float)NodeData["AttackDamage"]);
    player.AttackDamage = _player.AttackDamage;
    _player.Level = (int)((float)NodeData["Level"]);
    player.Level = _player.Level;
    _player.CurrentHealth = (int)((float)NodeData["CurrentHealth"]);
    player.CurrentHealth = _player.CurrentHealth;
    _player.MaxHealth = (int)((float)NodeData["MaxHealth"]);
    player.MaxHealth = _player.MaxHealth;
    _player.Experience = (int)((float)NodeData["Experience"]);
    player.Experience = _player.Experience;
    _player.Luck = (int)((float)NodeData["Luck"]);
    player.Luck = _player.Luck;
    _player.Intelligence = (int)((float)NodeData["Intelligence"]);
    player.Intelligence = _player.Intelligence;
    _player.Vitality = (int)((float)NodeData["Vitality"]);
    player.Vitality = _player.Vitality;
    _player.Dexterity = (int)((float)NodeData["Dexterity"]);
    player.Dexterity = _player.Dexterity;
    _player.Strength = (int)((float)NodeData["Strength"]);
    player.Strength = _player.Strength;
    Vector2 newPosition = new Vector2((float)NodeData["PosX"], (float)NodeData["PosY"]);
    _globalPlayer.PlayerLocation = newPosition;
    player.Position = newPosition;
    player.playerSpriteNode.FlipH = (Boolean)NodeData["facingLeft"];
    //Label healthLabel = (Label)GetNode("Player/Camera2D/CanvasLayer/HealthLabel");
    //healthLabel.Text = (string)nodeData["hplabel"];

    //var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
    //var newObject = (Node)newObjectScene.Instance();
    //GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
    //newObject.Set("Position", new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));
    NodeData = null;
  }
}

