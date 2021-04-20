using Godot;
using System;

public class Interaction : Node2D
{
    //Code inspired by GameDevelopmentCenter on YouTube

    //The dialogue box scene for use in code
    PackedScene dialogueBoxes = GD.Load<PackedScene>("res://DialogueBoxV2.tscn");

    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    //Enum states = {Idle, Looting, Reading, Puzzle, Secret}
    String action_state = "off";

    //The loot_areas to be disabled after the chest opens
    Godot.Collections.Array<Area2D> loot_areas = new Godot.Collections.Array<Area2D>();

    //The collsion shape to turn off
    CollisionShape2D disable_chest;

    //CanvasLayer to hold dialogue box
    DialogBox diagBox;

    // Chests are loaded from left to right, top to bottom. 
    // Chests in the upper left corner are loaded before lower right.
    // Tracking number of chests.
    int current_chest = 0;

    //Variables to hold current tilemap, tile texture and tile location
    TileMap changeMap;
    Vector2 tile;
    Vector2 tile_region;
    Vector2 open_chest = new Vector2(2,0);
    Godot.Collections.Array<Vector2> usedTiles = new Godot.Collections.Array<Vector2>();
    Godot.Collections.Array<Vector2> usedTextures = new Godot.Collections.Array<Vector2>();

    //Empty constructor for use in other scripts
    public Interaction() {

    }

    //Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        changeMap = (TileMap)GetNode("../Interactables");
        //CanvasLayer dialogBoxControl = (CanvasLayer)dialogueBoxes.Instance();
        //diagBox = dialogBoxControl.GetNode("DialogueBoxContainer") as DialogBox;
        diagBox = dialogueBoxes.Instance() as DialogBox;
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
                    diagBox.dialogPath = "res://Dialogues/Chest.txt";
                    AddChild(diagBox);
                    changeMap.SetCell((int)tile[0], (int)tile[1], 22, false, false, false, tile_region);
                    //SetCell(int x, int y, int tile, boolean flip_x, boolean flip_y, boolean transpose, Vector2 autotileCoordinates)

                    disable_chest = (CollisionShape2D)(GetNode(((String)(loot_areas[current_chest].GetPath() + "/CollisionShape2D"))));
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

    //The actual vector2 tile coordiate on the tilemap
    public void addUsedTiles(Vector2 used_tile) {
        usedTiles.Add(used_tile);
    }

    //The vector2 of tile texture from the tileset
    public void addUsedTextures(Vector2 used_texture) {
        usedTextures.Add(used_texture);
    }

    //The loot areas that will be disabled once the chest is opened
    public void setDisabledLootArea(Area2D openedChest) {
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