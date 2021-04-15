using Godot;
using System;

public class Inventory : Control
{
    private RichTextLabel _statText;
    private Item _heldItem;
    private GridContainer _invMenu;
    private GlobalPlayer _globalPlayer;

    public override void _Ready()
    {
        _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
        _statText = (RichTextLabel)GetNode("TextureRect").GetNode("StatText");
        _invMenu = (GridContainer)GetNode("TextureRect").GetNode("GridContainer");
        if (_globalPlayer._equipedArmor != null)
        {
            PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
            Item tempItem = (Item)ItemScene.Instance();
            tempItem.giveProperties(_globalPlayer._equipedArmor._name, _globalPlayer._equipedArmor._type, _globalPlayer._equipedArmor._stat, _globalPlayer._equipedArmor._bonus);
            FindNode("EquipedArmor").AddChild(tempItem);
            FindNode("EquipedArmor").Set("item", tempItem);
        }
        if (_globalPlayer._equipedWeapon != null)
        {
            PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
            Item tempItem = (Item)ItemScene.Instance();
            tempItem.giveProperties(_globalPlayer._equipedWeapon._name, _globalPlayer._equipedWeapon._type, _globalPlayer._equipedWeapon._stat, _globalPlayer._equipedWeapon._bonus);
            FindNode("EquipedWeapon").AddChild(tempItem);
            FindNode("EquipedWeapon").Set("item", tempItem);
        }
        if (_globalPlayer._inventory.Count > 0)
        {
            foreach (Item item in _globalPlayer._inventory)
            {
                PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
                Item tempItem = (Item)ItemScene.Instance();
                tempItem.giveProperties(item._name, item._type, item._stat, item._bonus);
                fillSlot(tempItem);
            }
        }
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
        foreach (Node slot in _invMenu.GetChildren())
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
        _statText.Text = "Your Stats: \n";
        _statText.Text += "Strength: " + _globalPlayer.Strength;
        _statText.Text += "\nDexterity: " + _globalPlayer.Dexterity;
        _statText.Text += "\nVitality: " + _globalPlayer.Vitality;
        _statText.Text += "\nIntelligence: " + _globalPlayer.Intelligence;
        _statText.Text += "\nLuck: " + _globalPlayer.Luck;
    }
}
