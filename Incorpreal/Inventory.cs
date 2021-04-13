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

    public void fillSlot(Item item)
    {
        foreach (Node slot in GetNode("TextureRect").GetNode("GridContainer").GetChildren())
        {
            if (slot.Get("item") == null)
            {
                slot.Set("item", item);
                slot.AddChild(item);
                break;
            }
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
