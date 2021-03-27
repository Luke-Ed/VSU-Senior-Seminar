using Godot;
using System;
using Incorpreal.TileSets;

//Code inspired by GameDevelopmentCenter on YouTube

public class Interactables : TileMap
{
    //Load the Area2D scene that represents lootable areas
    PackedScene loot_area = GD.Load<PackedScene>("res://TileSets/LootArea1.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Establish Signal Connection with CanvasLayer elements
        //This allows us to print to console or give other responses to Signals and Responses
        Interaction inter = GetNode("../Interaction_Console") as Interaction; //This miracle line of code says "load node A as a script"

        //Vector2 to represent 'Closed' chest textures
        Vector2 closed_chest_silver = new Vector2(0, 0);
        Vector2 closed_chest_gold = new Vector2(1, 0);

        //Return an array of all cells (array of objects) with the given tile id
        Godot.Collections.Array used_chest_tiles = GetUsedCellsById(22);

        //Loop through the above array to check if the chest is using the 'Closed' chest textures
        foreach (Vector2 cur_tile in used_chest_tiles)
        {
            if (GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_silver) || GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_gold)) {
                Area2D loot_area_instance = (Area2D)loot_area.Instance();
                loot_area_instance.Position = MapToWorld(cur_tile);
                //MapToWorld(Vector2) - Returns the global position corresponding to the given tilemap's (grid-based) coordinates.

                inter.AddUsedTiles((Vector2)cur_tile);
                inter.AddUsedTextures((Vector2)GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]));
                //GetCellAutoTileCoord(int x, int y) - Returns a Vector2 coordinate of the tile using a specific texture in the current tileset

                AddChild(loot_area_instance);
            }
        }

        //Get all nodes under the "LootAreas" Group
        //GetTree gets the SceneTree object that the node is in
        //GetNodesInGroup retrieves all nodes from a SceneTree that have the supplied group tag
        Godot.Collections.Array signal_emitters = GetTree().GetNodesInGroup("LootAreas");

        //Connect the appropriate signals to each "LootArea"
        //Signals are "body_entered" and "body_exited"
        foreach (Area2D area in signal_emitters)
        {
            area.Connect("body_entered", inter, "OnLootAreaEntered");
            area.Connect("body_exited", inter, "OnLootAreaExited");
            //Connect(String signal, Node signal_handler, String method_name)

            inter.SetDisabledLootArea(area);
        }
    }
}