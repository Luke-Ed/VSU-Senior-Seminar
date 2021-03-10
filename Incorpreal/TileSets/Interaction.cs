using Godot;
using System;

public class Interaction : CanvasLayer
{
    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    String action_state = "off";

    //

    //Variables to hold current tile texture and location
    Vector2 tile;
    Vector2 tile_region;
    Vector2 flip_tile = new Vector2(2,0);
    Godot.Collections.Array<Vector2> usedTiles = new Godot.Collections.Array<Vector2>();

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

    public void setUsedTiles(Godot.Collections.Array<Vector2> tiles) {
        usedTiles = tiles;
    }
    
    //When the player enters a loot area, print "Looting"
    public void OnLootAreaEntered(Area2D area) {
        action_state = "on";
        tile = (Vector2)usedTiles[0];
        tile_region = (Vector2)usedTiles[1] + flip_tile;
        GD.Print("Looting");
    }

    //When the player leaves a loot area, print "Leaving"
    public void OnLootAreaExited(Area2D area) {
        action_state = "off";
        GD.Print("Leaving");
    }
}