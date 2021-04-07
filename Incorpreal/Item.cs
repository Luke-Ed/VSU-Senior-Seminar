using Godot;
using System;

public class Item : Node
{
    private String _name;
    private String _type;
    private String _stat;
    private int _bonus;
    private GlobalPlayer _globalPlayer;


    // Making a new object would look like "Item i = new Item("Long Sword", "Weapon", "Strength", 5);" When equiped this would increase the players strength score by 5.
    public Item(String name, String type, String stat, int bonus)
    {
        this._name = name;
        this._type = type;
        this._stat = stat;
        this._bonus = bonus;
        _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
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
                break;
            case ("Armor"):
                if (_globalPlayer._equipedArmor != null)
                {
                    _globalPlayer._equipedArmor.unequip();
                }
                _globalPlayer._equipedArmor = this;
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
        }
        else
        {
            _globalPlayer._equipedArmor = null;
        }
    }
}
