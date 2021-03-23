using Godot;
using System;

public class Interaction : CanvasLayer
{
    //Code inspired by GameDevelopmentCenter on YouTube
    
    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    String action_state = "off";

    //The loot_area to be disabled after interaction
    //Area2D loot_area_import;
    Godot.Collections.Array<Area2D> loot_areas = new Godot.Collections.Array<Area2D>();
    
    /*
    Chests are loaded from left to right, top to bottom. 
    Chests in the upper left corner are loaded before lower right.
    I should be able to work backward from an integer count of how many chests
    are on the map if I am extremely careful
    int chest_count = 0;
    */

    //Variables to hold current tile texture and location
    Vector2 tile;
    Vector2 tile_region;
    Vector2 open_chest = new Vector2(2,0);
    Godot.Collections.Array<Vector2> usedTiles = new Godot.Collections.Array<Vector2>();
    Godot.Collections.Array<Vector2> usedTextures = new Godot.Collections.Array<Vector2>();

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
    public void addUsedTiles(Vector2 used_tile) {
        usedTiles.Add(used_tile);
    }

    public void addUsedTextures(Vector2 used_texture) {
        usedTextures.Add(used_texture);
    }

    public void setDisabledLootArea(Area2D openedChest) {
        //loot_area_import = openedChest;
        loot_areas.Add(openedChest);
    }
    
    //When the player enters a loot_area, print "Looting"
    public void OnLootAreaEntered(Area2D area) {
        action_state = "on";
        tile = (Vector2)usedTiles[0]; //Pass the current tile ingame to the process method
        tile_region = (Vector2)usedTextures[0] + open_chest; //Pass the Vector2 and change the texture to the "open chest" texture
        GD.Print("Looting");
    }

    //When the player leaves a loot_area, print "Leaving"
    public void OnLootAreaExited(Area2D area) {
        action_state = "off";
        GD.Print("Leaving");
    }
}