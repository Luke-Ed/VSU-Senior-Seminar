using Godot;
using System;

public class SaveLoadGame : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Save() {
        var saveFile = new File();
        saveFile.Open("user://savegame.save", File.ModeFlags.Write);

        var saveables = GetTree().GetNodesInGroup("saveable");
        foreach (Node saveable in saveables) {
            if (saveable.Filename.Empty()) {
                GD.Print(String.Format("node '{0}' is not an instanced scene", saveable.Name));
                continue;
            }
            if (!saveable.HasMethod("Save")) {
                GD.Print("node '{0}' has no Save method", saveable.Name));
                continue;
            }
            var saveData = saveable.Call("Save");
            saveFile.StoreLine(JSON.Print(saveData));
        }
        saveFile.Close();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
