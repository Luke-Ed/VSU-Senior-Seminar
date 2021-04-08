using System;
using Godot;

namespace Incorpreal.TileSets {
  public class Interaction : CanvasLayer {
    //Code inspired by GameDevelopmentCenter on YouTube
    
    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    //Enum states = {Idle, Looting, Reading, Puzzle, Secret}
    String action_state = "off";

    //The loot_areas to be disabled after the chest opens
    Godot.Collections.Array<Area2D> loot_areas = new Godot.Collections.Array<Area2D>();
    
    
    // Chests are loaded from left to right, top to bottom. 
    // Chests in the upper left corner are loaded before lower right.
    // Tracking passed textures works out better than number of chests!
    int current_chest = 0;

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
                    GD.Print("I got nothing to do here. \n");
                    break;

                case "on":
                    //Add some function or load a scene/panel for gold, etc.
                    GD.Print("What do we have here? \n");
                    TileMap changeMap = (TileMap)GetNode("../Interactables");
                    changeMap.SetCell((int)tile[0], (int)tile[1], 22, false, false, false, tile_region);
                    //SetCell(int x, int y, int tile, boolean flip_x, boolean flip_y, boolean transpose, Vector2 autotileCoordinates)

                    CollisionShape2D disable_chest = (CollisionShape2D)(GetNode(((String)(loot_areas[current_chest].GetPath() + "/CollisionShape2D"))));
                    disable_chest.Disabled = true;
                    //Disable the collision box for this chest

                    current_chest++;
                    //Switch to next chest
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
        tile = (Vector2)usedTiles[current_chest]; //Pass the current tile ingame to the process method
        tile_region = (Vector2)usedTextures[current_chest] + open_chest; //Pass the Vector2 and change the texture to the "open chest" texture
        // GD.Print("Looting \n");
    }

    //When the player leaves a loot_area, print "Leaving"
    public void OnLootAreaExited(Area2D area) {
        action_state = "off";
        // GD.Print("Leaving \n");
    }
  }
}