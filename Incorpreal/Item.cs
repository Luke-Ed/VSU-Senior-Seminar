using Godot;
using System;

public class Item : Node
{
    public String _name { get; set; }
    public String _type { get; set; }
    public String _stat { get; set; }
    public int _bonus { get; set; }
    private GlobalPlayer _globalPlayer;
    private TextureRect _itemPicture;

    public Item()
    {

    }


    // Needed to make this method due to the fact that you cannot just create an item with stats in the same method as the constructor due to having to pack the scene first in order to make it an
    // interactable object within the inventory screne.
    public void giveProperties(String name, String type, String stat, int bonus)
    {
        this._name = name; //Any name you like
        this._type = type; // "Weapon", "Armor", or "Consumable" for now.
        this._stat = stat; // "Strength", "Dexterity", "Vitality", "Intelligence", or "Luck"
        this._bonus = bonus; //Positive number.
    }
    public override void _Ready()
    {
        _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
        //just for testing adding gem to each slot
        _itemPicture = (TextureRect)GetNode("Picture");
        _itemPicture.Texture = (Texture)ResourceLoader.Load("res://assets/gem.png");
    }

    public void equip()
    {
        switch (_type)
        {
            case ("Weapon"):
                if (_globalPlayer._equipedWeapon != null)
                {
                    _globalPlayer._equipedWeapon.unequip();
                }
                _globalPlayer._equipedWeapon = this;
                _globalPlayer._inventory.Remove(this);
                break;
            case ("Armor"):
                if (_globalPlayer._equipedArmor != null)
                {
                    _globalPlayer._equipedArmor.unequip();
                }
                _globalPlayer._equipedArmor = this;
                _globalPlayer._inventory.Remove(this);
                break;
            default:
                break;
        }
        switch (_stat)
        {
            case ("Strength"):
                _globalPlayer.Strength += _bonus;
                break;
            case ("Dexterity"):
                _globalPlayer.Dexterity += _bonus;
                break;
            case ("Vitality"):
                _globalPlayer.Vitality += _bonus;
                break;
            case ("Intelligence"):
                _globalPlayer.Intelligence += _bonus;
                break;
            case ("Luck"):
                _globalPlayer.Luck += _bonus;
                break;
            default:
                break;
        }
    }

    private void unequip()
    {
        switch (_stat)
        {
            case ("Strength"):
                _globalPlayer.Strength -= _bonus;
                break;
            case ("Dexterity"):
                _globalPlayer.Dexterity -= _bonus;
                break;
            case ("Vitality"):
                _globalPlayer.Vitality -= _bonus;
                break;
            case ("Intelligence"):
                _globalPlayer.Intelligence -= _bonus;
                break;
            case ("Luck"):
                _globalPlayer.Luck -= _bonus;
                break;
            default:
                break;
        }
        if (_type.Equals("Weapon")){
            _globalPlayer._equipedWeapon = null;
            _globalPlayer._inventory.Add(this);
        }
        else
        {
            _globalPlayer._equipedArmor = null;
            _globalPlayer._inventory.Add(this);
        }
    }
}
