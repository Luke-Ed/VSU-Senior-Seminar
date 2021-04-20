using Godot;
using System;

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
        Interaction outConsole = GetTree().Root.GetNode("Node2D/Interaction_Console") as Interaction; //This miracle line of code says "load node A as a script"

        //Vector2 to represent the signs in the Forest Map
        //Vector2 readable_sign = new Vector2(2,1);

        //Vector2 to represent 'Closed' chest textures
        Vector2 chestSilver = new Vector2(0, 0);
        Vector2 chestGold = new Vector2(1, 0);

        //Return an array of all cells (array of objects) with the given tile id
        Godot.Collections.Array chestsOnMap = GetUsedCellsById(22);
        //Godot.Collections.Array used_sign_tiles = GetUsedCellsById(21);

        //Loop through the above array to check if the chest is using the 'Closed' chest textures
        foreach (Vector2 currentTile in chestsOnMap)
        {
            if (GetCellAutotileCoord((int)currentTile[0], (int)currentTile[1]).Equals(chestSilver) || GetCellAutotileCoord((int)currentTile[0], (int)currentTile[1]).Equals(chestGold)) {
                Area2D lootAreaInstance = (Area2D)loot_area.Instance();
                lootAreaInstance.Position = MapToWorld(currentTile);
                //MapToWorld(Vector2) - Returns the global position corresponding to the given tilemap's (grid-based) coordinates.

                outConsole.AddChestTile((Vector2)currentTile);
                outConsole.AddChestTextures((Vector2)GetCellAutotileCoord((int)currentTile[0], (int)currentTile[1]));
                //GetCellAutoTileCoord(int x, int y) - Returns a Vector2 coordinate of the tile using a specific texture in the current tileset

                AddChild(lootAreaInstance);
            }
        }

        //Get all nodes under the "LootAreas" Group
        //GetTree gets the SceneTree object that the node is in
        //GetNodesInGroup retrieves all nodes from a SceneTree that have the supplied group tag
        Godot.Collections.Array signalEmitters = GetTree().GetNodesInGroup("LootAreas");

        //Connect the appropriate signals to each "LootArea"
        //Signals are "body_entered" and "body_exited"
        foreach (Area2D area in signalEmitters)
        {
            area.Connect("body_entered", outConsole, "OnLootAreaEntered");
            area.Connect("body_exited", outConsole, "OnLootAreaExited");
            //Connect(String signal, Node signal_handler, String method_name)

            outConsole.AddOpenedLootAreas(area);
        }
    }
}