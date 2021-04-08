using Godot;
using System;

public class Slot : Panel
{
    private Item item = null;
    private PackedScene ItemScene;

    public override void _Ready()
    {
        ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
        item = (Item)ItemScene.Instance();
        AddChild(item);
    }


}
