using Godot;
using System;

public class Slot : Panel
{
    private Item item = null;
    private PackedScene ItemScene;
    private RichTextLabel _statText;

    public override void _Ready()
    {
        //Note all of this code is just for example and testing by making two test items and filling them out throughout the inventory. Will not be in the final product.
        _statText = (RichTextLabel)GetParent().GetParent().GetNode("StatText");
        ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
        item = (Item)ItemScene.Instance();
        Random random = new Random();
        int roll = random.Next(2);
        if (roll == 0)
        {
            item._name = "Test Item1";
            item._type = "Weapon";
            item._stat = "Luck";
            item._bonus = 5;
        }
        else
        {
            item._name = "Test Item2";
            item._type = "Armor";
            item._stat = "Strength";
            item._bonus = 2;
        }
        AddChild(item);
    }

    private void _on_Slot_mouse_entered()
    {
        _statText.Text = "Name: " + item._name;
        _statText.Text += "\nType: " + item._type;
        _statText.Text += "\n" + item._stat + " + " + item._bonus;
    }




}
