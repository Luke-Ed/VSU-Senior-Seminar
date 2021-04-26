using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SaveLoadGame : Node
{
    public GlobalPlayer gp;
    public Godot.Collections.Dictionary<string, object> nodeData_;
    public override void _Ready()
    {
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    public Boolean Save(Godot.Collections.Array saveables)
    {
        var saveFile = new File();
        saveFile.Open("user://savegame.save", File.ModeFlags.Write); //Open file in write mode

        foreach (Node saveable in saveables)
        { //Iterate them
            if (saveable.Filename.Empty())
            { //Skip empty nodes
                GD.Print(String.Format("node '{0}' is not an instanced scene ", saveable.Name));
                continue;
            }
            if (!saveable.HasMethod("Save"))
            { //Skip ones without save methods
                GD.Print("node '{0}' has no Save method", saveable.Name);
                continue;
            }
            var saveData = saveable.Call("Save");
            saveFile.StoreLine(JSON.Print(saveData));
        }

        saveFile.Close();
        return true;
    }

    public System.Action Unload(File saveFile, Godot.Collections.Dictionary<string, object> nodeData) 
    {
        //First free previous resources
        //foreach (Node node in GetTree().Root.GetChildren()) {
         //   node.QueueFree();
        //}
        this.nodeData_ = nodeData;

        //Check which level its in
        string level = (string) nodeData["currentLevel"];

        //Load the level
        GetTree().ChangeScene("res://levels/" + level + ".tscn");

        foreach (Node node in GetTree().Root.GetChildren()) {
            GD.Print("Before: " + node, node.Name);
            if (node.Name == "Level 1") {
                var newTree = node.GetChildren();
                foreach (Node newNode in newTree) {
                    GD.Print("Before: Level 1: " + newNode + " " + newNode.Name);
                }
            }
        }
        saveFile.Close();
        System.Action loadAction = new System.Action(Load);
        return loadAction;
    }

    public void Load() {
        //Grab the player node
        Player player = (Player)GetTree().Root.GetNode("Level 1/Player");

        //Check if the player is possessing someone or not
        if (nodeData_["resPath"] != null) {
            player.resPath = (string)nodeData_["resPath"];
            player.playerSpriteNode.Texture.ResourcePath = (string)nodeData_["playerSpriteNode.Texture.ResourcePath"];
            player.PossesseeName = (string)nodeData_["PossesseeName"];
            gp.isPossesing = true;
        } else {
            gp.isPossesing = false;
        }

        //Reload all the necessary values
        player.moveSpeed = (int)((float)nodeData_["moveSpeed"]);
        player.stuck = (Boolean)nodeData_["stuck"];
        gp.currentPoints = (int)((float)nodeData_["currentPoints"]);
        gp.spiritPoints = (int)((float)nodeData_["spiritPoints"]);
        gp.baseStat = (int)((float)nodeData_["baseStat"]);
        gp.ExperienceToNextLevel = (int)((float)nodeData_["ExperienceToNextLevel"]);
        player.ExperienceToNextLevel = gp.ExperienceToNextLevel;
        gp.AttackDamage = (int)((float)nodeData_["AttackDamage"]);
        player.AttackDamage = gp.AttackDamage;
        gp.Level = (int)((float)nodeData_["Level"]);
        player.Level = gp.Level;
        gp.CurrentHealth = (int)((float)nodeData_["CurrentHealth"]);
        player.CurrentHealth = gp.CurrentHealth;
        gp.MaxHealth = (int)((float)nodeData_["MaxHealth"]);
        player.MaxHealth = gp.MaxHealth;
        gp.Experience = (int)((float)nodeData_["Experience"]);
        player.Experience = gp.Experience;
        gp.Luck = (int)((float)nodeData_["Luck"]);
        player.Luck = gp.Luck;
        gp.Intelligence = (int)((float)nodeData_["Intelligence"]);
        player.Intelligence = gp.Intelligence;
        gp.Vitality = (int)((float)nodeData_["Vitality"]);
        player.Vitality = gp.Vitality;
        gp.Dexterity = (int)((float)nodeData_["Dexterity"]);
        player.Dexterity = gp.Dexterity;
        gp.Strength = (int)((float)nodeData_["Strength"]);
        player.Strength = gp.Strength;
        Vector2 newPosition = new Vector2((float)nodeData_["PosX"], (float)nodeData_["PosY"]);
        player.Set("Position", newPosition);
        gp.playerLocation = newPosition;
        foreach (Node node in GetTree().Root.GetChildren()) {
            GD.Print("After: " + node, node.Name);
            if (node.Name == "Level 1") {
                var newTree = node.GetChildren();
                foreach (Node newNode in newTree) {
                    GD.Print("After: Level 1: " + newNode + " " + newNode.Name);
                }
            }
        }
        //Label healthLabel = (Label)GetNode("Player/Camera2D/CanvasLayer/HealthLabel");
        //healthLabel.Text = (string)nodeData["hplabel"];

        //var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
        //var newObject = (Node)newObjectScene.Instance();
        //GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
        //newObject.Set("Position", new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));
    }
}