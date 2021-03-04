using Godot;
using System;

public class Interactables : TileMap
{
    //Establish loot area
    PackedScene loot_area = GD.Load<PackedScene>("res://TileSets/LootArea1.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Code inspired by GameDevelopmentCenter on YouTube
        //Vector2 to represent 'Closed' chest texture
        Vector2 closed_chest_silver = new Vector2(0, 0);
        Vector2 closed_chest_gold = new Vector2(2, 0);

        //Return an array of all cells with the given tile id
        Godot.Collections.Array used_chest_tiles = GetUsedCellsById(22);

        //Loop through each Vector2 to check if the chest is using the 'Closed' texture
        foreach (Vector2 cur_tile in used_chest_tiles)
        {
            if (GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_silver) || GetCellAutotileCoord((int)cur_tile[0], (int)cur_tile[1]).Equals(closed_chest_gold)) {
                Area2D loot_area_instance = (Area2D)loot_area.Instance();
                loot_area_instance.Position = MapToWorld(cur_tile);
                AddChild(loot_area_instance);
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
