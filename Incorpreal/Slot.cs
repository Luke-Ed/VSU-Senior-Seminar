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
    }

    private void _on_Slot_mouse_entered()
    {
        if (item != null)
        {
            _statText.Text = "Name: " + item._name;
            _statText.Text += "\nType: " + item._type;
            _statText.Text += "\n" + item._stat + " + " + item._bonus;
        }
    }


}
