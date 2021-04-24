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
        Godot.Collections.Dictionary<string, object> nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveFile.GetLine()).Result); //Read next line from file
        string level = (string) nodeData["currentLevel"];

        //Load the appropriate scene for the level, no need to unload old scene as it is done automatically
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

        //Check if the player is possessing someone or not
        if (nodeData["resPath"] != null) {
            player.resPath = (string)nodeData["resPath"];
            player.playerSpriteNode.Texture.ResourcePath = (string)nodeData["playerSpriteNode.Texture.ResourcePath"];
            player.PossesseeName = (string)nodeData["possesseeName"];
            gp.isPossesing = true;
        } else {
            gp.isPossesing = false;
        }

        //Reload all the necessary values
        player.moveSpeed = (int)nodeData["moveSpeed"];


            //var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
            //var newObject = (Node)newObjectScene.Instance();
            //GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
            //newObject.Set("Position", new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));
        saveFile.Close();
    }
}