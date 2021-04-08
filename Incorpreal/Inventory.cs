using Godot;
using System;

public class Inventory : Control
{


    public override void _Ready()
    {

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
}
