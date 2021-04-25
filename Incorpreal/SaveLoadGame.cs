using Godot;
using System;
using System.Collections.Generic;

public class SaveLoadGame : Node
{
    public GlobalPlayer gp;
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

    public void Load(File saveFile, Godot.Collections.Array saveNodes) 
    {
        //Read the save file
        Godot.Collections.Dictionary<string, object> nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveFile.GetLine()).Result); //Read next line from file
        
        //Check which level its in
        string level = (string) nodeData["currentLevel"];

        //Load that level, old scene's resources automatically freed
        switch ((string)nodeData["currentLevel"]) {
            case "Level 1":
                GetTree().ChangeScene("res://levels/Level 1.tscn");
                break;
            case "Level 2":
                GetTree().ChangeScene("res://levels/Level 2.tscn");
                break;
            case "Level 3":
                GetTree().ChangeScene("res://levels/Level 3.tscn");
                break;
            default:
                break;
        }

        //Grab the player node
        var persistGroup = GetTree().GetNodesInGroup("persist");
        Player player = (Player) persistGroup[0];

        //Check if the player is possessing someone or not
        if (nodeData["resPath"] != null) {
            player.resPath = (string)nodeData["resPath"];
            player.playerSpriteNode.Texture.ResourcePath = (string)nodeData["playerSpriteNode.Texture.ResourcePath"];
            player.PossesseeName = (string)nodeData["PossesseeName"];
            gp.isPossesing = true;
        } else {
            gp.isPossesing = false;
        }

        //Reload all the necessary values
        player.moveSpeed = (int)((float)nodeData["moveSpeed"]);



        player.stuck = (Boolean)nodeData["stuck"];
        gp.currentPoints = (int)((float)nodeData["currentPoints"]);
        gp.spiritPoints = (int)((float)nodeData["spiritPoints"]);
        gp.baseStat = (int)((float)nodeData["baseStat"]);
        gp.ExperienceToNextLevel = (int)((float)nodeData["ExperienceToNextLevel"]);
        player.ExperienceToNextLevel = gp.ExperienceToNextLevel;
        gp.AttackDamage = (int)((float)nodeData["AttackDamage"]);
        player.AttackDamage = gp.AttackDamage;
        gp.Level = (int)((float)nodeData["Level"]);
        player.Level = gp.Level;
        gp.CurrentHealth = (int)((float)nodeData["CurrentHealth"]);
        player.CurrentHealth = gp.CurrentHealth;
        gp.MaxHealth = (int)((float)nodeData["MaxHealth"]);
        player.MaxHealth = gp.MaxHealth;
        gp.Experience = (int)((float)nodeData["Experience"]);
        player.Experience = gp.Experience;
        gp.Luck = (int)((float)nodeData["Luck"]);
        player.Luck = gp.Luck;
        gp.Intelligence = (int)((float)nodeData["Intelligence"]);
        player.Intelligence = gp.Intelligence;
        gp.Vitality = (int)((float)nodeData["Vitality"]);
        player.Vitality = gp.Vitality;
        gp.Dexterity = (int)((float)nodeData["Dexterity"]);
        player.Dexterity = gp.Dexterity;
        gp.Strength = (int)((float)nodeData["Strength"]);
        player.Strength = gp.Strength;
        gp.playerLocation = new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]);
        player.Position = gp.playerLocation;
        //Label healthLabel = (Label)GetNode("Player/Camera2D/CanvasLayer/HealthLabel");
        //healthLabel.Text = (string)nodeData["hplabel"];

        //var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
        //var newObject = (Node)newObjectScene.Instance();
        //GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
        //newObject.Set("Position", new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));
        saveFile.Close();
    }
}