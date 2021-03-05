using Godot;
using System;

public class Interaction : CanvasLayer
{
    //Our "state" for player interaction with map objects
    String action_state = "off";

    public override void _Process(float delta) {
        if(Input.IsActionJustPressed("interact")) {
            //Respond based on the action_state
            switch(action_state) {
                case "off":
                    GD.Print("I got nothing to do here.");
                    break;
                case "on":
                    //Add some function or load a scene/panel for gold, etc.
                    GD.Print("What do we have here?");
                    break;
                default:
                    GD.Print("...");
                    break;
            }
        }
    }
    
    //When the player enters a loot area, print "Looting"
    public void OnLootAreaEntered(Area2D area) {
        action_state = "on";
        GD.Print("Looting");
    }

    //When the player leaves a loot area, print "Leaving"
    public void OnLootAreaExited(Area2D area) {
        action_state = "off";
        GD.Print("Leaving");
    }
}