using Godot;
using System;

//Code inspired by GameDevelopmentCenter on YouTube

public class Interactables : TileMap
{
    //Establish loot area
    PackedScene loot_area = GD.Load<PackedScene>("res://TileSets/LootArea1.tscn");

    //Array to hold the literal tile and the texture on that tile
    Godot.Collections.Array<Vector2> usedTileAndTextures = new Godot.Collections.Array<Vector2>();

    Interaction conPrint = new Interaction();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Establish Signal Connection with CanvasLayer elements
        CanvasLayer output_console = (CanvasLayer)GetNode("../Interaction_Console");
        Interaction inter = GetNode("../Interaction_Console") as Interaction;

        //Vector2 to represent 'Closed' chest texture
        Vector2 closed_chest_silver = new Vector2(0, 0);
        Vector2 closed_chest_gold = new Vector2(2, 0);

        //Return an array of all cells (array of Vector2) with the given tile id
        Godot.Collections.Array used_chest_tiles = GetUsedCellsById(22);

        //Loop through each Vector2 to check if the chest is using the 'Closed' textures
        foreach (Vector2 cur_tile in used_chest_tiles)
        {
            if (GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_silver) || GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_gold)) {
                Area2D loot_area_instance = (Area2D)loot_area.Instance();
                loot_area_instance.Position = MapToWorld(cur_tile);
                usedTileAndTextures.Add((Vector2)cur_tile);
                usedTileAndTextures.Add((Vector2)GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]));
                AddChild(loot_area_instance);
            }
        }

        //Get all nodes under the "LootAreas" Group
        //GetTree gets the SceneTree object that the node is in
        //GetNodesInGroup retrieves all nodes from a SceneTree that have the supplied group tag
        Godot.Collections.Array signal_radius = GetTree().GetNodesInGroup("LootAreas");

        //Connect the appropriate signals to each "LootArea"
        //Signals are "body_entered" and "body_exited"
        foreach (Area2D area in signal_radius)
        {
            area.Connect("body_entered", output_console, "OnLootAreaEntered");
            area.Connect("body_exited", output_console, "OnLootAreaExited");
        }

        inter.setUsedTiles(usedTileAndTextures);
    }
}