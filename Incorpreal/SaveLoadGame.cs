using Godot;
using System;
using System.Collections.Generic;

public class SaveLoadGame : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void Save(Godot.Collections.Array saveables)
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
    }

    public void Load()
    { //Not nearly finished
        var saveFile = new File();

        if (!saveFile.FileExists("user://savegame.save"))
        { //If none found
            GD.Print("Cannot find a save file in %s", OS.GetUserDataDir());
            return; //Stop loading
        }
        else
        {
            saveFile.Open("user://savegame.save", File.ModeFlags.Read);

            //Get rid of current persistant nodes before loading
            var saveNodes = GetTree().GetNodesInGroup("persist");
            foreach (Node saveNode in saveNodes)
            {
                saveNode.QueueFree();
            }

            while (saveFile.GetPosition() < saveFile.GetLen())
            { //While there is still file left to read
                var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveFile.GetLine()).Result); //Read next line from file

                var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
                var newObject = (Node)newObjectScene.Instance();
                GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
                newObject.Set("Position", new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));

                /*   foreach(KeyValuePair<string, string> entry in nodeData) {
                       string key = entry.Key.ToString();
                       if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY") {
                           continue;
                       }
                       newObject.Set(key, entry.Value);
                   }
               }*/
                saveFile.Close();
            }

        }
        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}