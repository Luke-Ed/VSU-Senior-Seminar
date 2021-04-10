using Godot;
using System;

public class Inventory : Control
{
    private RichTextLabel _statText;

    public override void _Ready()
    {
        _statText = (RichTextLabel)GetNode("TextureRect").GetNode("StatText");
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("inventory"))
        {
            openInventory();
        }
    }

    private void openInventory()
    {
        this.Visible = !this.Visible;
        GetTree().Paused = !GetTree().Paused;
    }

    private void _on_Slot_mouse_exited()
    {
        _statText.Text = "";
    }
}
