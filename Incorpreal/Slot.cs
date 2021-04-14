using Godot;
using System;

public class Slot : Panel
{
    private Item item = null;
    private PackedScene ItemScene;
    private RichTextLabel _statText;
    private Panel _equipedArmor, _equipedWeapon;
    private Node _invMenu;

    public override void _Ready()
    {
        _statText = (RichTextLabel)GetParent().GetParent().GetNode("StatText");
        _invMenu = FindParent("Inventory");
        _equipedArmor = (Panel)_invMenu.FindNode("EquipedArmor");
        _equipedWeapon = (Panel)_invMenu.FindNode("EquipedWeapon");
    }

    public void equipItem()
    {
        switch (item._type)
        {
            case ("Weapon"):
                if (_equipedWeapon.Get("item") != null)
                {
                    unequipItem((Item)_equipedWeapon.Get("item"));
                }
                RemoveChild(item);
                _equipedWeapon.AddChild(item);
                _equipedWeapon.Set("item", item);
                item.equip();
                item = null;
                break;
            case ("Armor"):
                if (_equipedArmor.Get("item") != null)
                {
                    unequipItem((Item)_equipedArmor.Get("item"));
                }
                RemoveChild(item);
                _equipedArmor.AddChild(item);
                _equipedArmor.Set("item", item);
                item.equip();
                item = null;
                break;
            case null:
                break;
        }
    }

    private void unequipItem(Item i)
    {
        switch (item._type)
        {
            case ("Weapon"):
                _equipedWeapon.RemoveChild(i);
                _equipedWeapon.Set("item", null);
                _invMenu.Call("fillSlot", i);
                break;
            case ("Armor"):
                _equipedArmor.RemoveChild(i);
                _equipedArmor.Set("item", null);
                _invMenu.Call("fillSlot", i);
                break;
            case null:
                break;
        }
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

    private void _on_Slot_gui_input(InputEvent @event)
    {
        if(@event is InputEventMouseButton && item != null)
        {
            if (IsInGroup("BaseInventory"))
            {
                equipItem();
            }
        }
    }

}
