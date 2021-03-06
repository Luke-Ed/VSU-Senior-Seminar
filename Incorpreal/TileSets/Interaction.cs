using Godot;
using System;

public class Interaction : CanvasLayer
{
    //Code inspired by GameDevelopmentCenter on YouTube
    
    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    String action_state = "off";

    //The loot_area to be disabled after interaction
    //Area2D loot_area_import = ;

    //Variables to hold current tile texture and location
    Vector2 tile;
    Vector2 tile_region;
    Vector2 open_chest = new Vector2(2,0);
    Godot.Collections.Array<Vector2> usedTiles = new Godot.Collections.Array<Vector2>();

    //Empty constructor for use in other scripts
    public Interaction() {

    }

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
                    TileMap changeMap = (TileMap)GetNode("../Interactables");
                    changeMap.SetCell((int)tile[0], (int)tile[1], 22, false, false, false, tile_region);
                    //SetCell(int x, int y, int tile, boolean flip_x, boolean flip_y, boolean transpose, Vector2 autotileCoordinates)
                    break;

                default:
                    GD.Print("...");
                    break;
            }
        }
    }

    //Please just ignore, it's just a setter
    public void setUsedTiles(Godot.Collections.Array<Vector2> used_tile) {
        usedTiles = (used_tile);
    }
    //Side note: maybe instead of getting the whole array, I can add each tile used to an array and work through each
    //That should aid the issue of multiplicity. (It only works for one chest on the map. I'm still working on it.)
    //I spent four days trying not to lose my mind with this GDScript tutorial and converting it to C#

    /*
    public void setDisabledLootArea(Area2D openedChest) {
        loot_area_import = openedChest;
    }
    */
    
    //When the player enters a loot_area, print "Looting"
    public void OnLootAreaEntered(Area2D area) {
        action_state = "on";
        tile = (Vector2)usedTiles[0]; //Pass the current tile ingame to the process method
        tile_region = (Vector2)usedTiles[1] + open_chest; //Pass the Vector2 and change the texture to the "open chest" texture
        GD.Print("Looting");
    }

    //When the player leaves a loot_area, print "Leaving"
    public void OnLootAreaExited(Area2D area) {
        action_state = "off";
        GD.Print("Leaving");
    }
}