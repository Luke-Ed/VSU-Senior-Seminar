using System;
using Godot;

namespace Incorpreal {
  public class Item : Node {
    public String ItemName { get; set; }
    public String Type { get; set; }
    public String Stat { get; set; }
    public int Bonus { get; set; }
    private GlobalPlayer _globalPlayer;
    public NodePath SpritePath { get; set; }
    private TextureRect _itemPicture;
    

    // Needed to make this method due to the fact that you cannot just create an item with stats in the same method as the constructor due to having to pack the scene first in order to make it an
    // interactable object within the inventory screne.
    public void GiveProperties(String name, String type, String stat, int bonus) {
      this.ItemName = name; //Any name you like
      this.Type = type; // "Weapon", "Armor", or "Consumable" for now.
      this.Stat = stat; // "Strength", "Dexterity", "Vitality", "Intelligence", or "Luck". (If it is a Consumable "Health" or "Spirit"
      this.Bonus = bonus; //Positive number.
    }
    public override void _Ready() {
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      //just for testing adding gem to each slot
    }

    public void changePicture(NodePath nodePath) {
      _itemPicture = (TextureRect)GetNode("Picture");
      SpritePath = nodePath;
      _itemPicture.Texture = (Texture)ResourceLoader.Load(nodePath);
    }

    public void equip() {
      //Takes the item type and places it in the globalplayer slot while removing it from the inventory of the player.
      switch (Type)
      {
        case ("Weapon"):
          if (_globalPlayer.EquippedWeapon != null) {
            _globalPlayer.EquippedWeapon.unequip();
          }
          _globalPlayer.EquippedWeapon = this;
          _globalPlayer.Inventory.Remove(this);
          break;
        case ("Armor"):
          if (_globalPlayer.EquippedArmor != null) {
            _globalPlayer.EquippedArmor.unequip();
          }
          _globalPlayer.EquippedArmor = this;
          _globalPlayer.Inventory.Remove(this);
          break;
        case ("Consumable"):
          //Potions as of now will increase your current health or spirit more options can be added at a later time if desired.
          if (Stat == "Health") {
            _globalPlayer.PlayerCharacter.CurrentHealth += Bonus;
            if (_globalPlayer.PlayerCharacter.CurrentHealth > _globalPlayer.PlayerCharacter.MaxHealth) {
              _globalPlayer.PlayerCharacter.CurrentHealth = _globalPlayer.PlayerCharacter.MaxHealth;
            }
          }
          else if (Stat == "Spirit") {
            _globalPlayer.PlayerCharacter.MaxSpiritPoints += Bonus;
            if (_globalPlayer.PlayerCharacter.CurrentSpiritPoints > _globalPlayer.PlayerCharacter.MaxSpiritPoints) {
              _globalPlayer.PlayerCharacter.CurrentSpiritPoints = _globalPlayer.PlayerCharacter.MaxSpiritPoints;
            }
          }
          _globalPlayer.updateHealthLabel(_globalPlayer.hplabel);
          _globalPlayer.Inventory.Remove(this);
          break;
      }
      //Increases the given stat and updates any other stat that is associated with it.
      switch (Stat) {
        case ("Strength"):
          _globalPlayer.PlayerCharacter.Strength += Bonus;
          _globalPlayer.PlayerCharacter.AttackDamage = _globalPlayer.BaseStat + _globalPlayer.PlayerCharacter.Strength;
          break;
        case ("Dexterity"):
          _globalPlayer.PlayerCharacter.Dexterity += Bonus;
          _globalPlayer.PlayerCharacter.AttackDamage = _globalPlayer.BaseStat + _globalPlayer.PlayerCharacter.Dexterity;
          break;
        case ("Vitality"):
          _globalPlayer.PlayerCharacter.Vitality += Bonus;
          _globalPlayer.PlayerCharacter.MaxHealth = _globalPlayer.BaseStat + _globalPlayer.PlayerCharacter.Vitality;
          break;
        case ("Intelligence"):
          _globalPlayer.PlayerCharacter.Intelligence += Bonus;
          _globalPlayer.PlayerCharacter.MaxSpiritPoints = _globalPlayer.BaseStat + _globalPlayer.PlayerCharacter.Intelligence;
          break;
        case ("Luck"):
          _globalPlayer.PlayerCharacter.Luck += Bonus;
          break;
      }
    }

    //Unequiping an item will remove any stat bonuses and remove the item from the given slot and add it back into the inventory list of global player.
    private void unequip() {
      switch (Stat) {
        case ("Strength"):
          _globalPlayer.PlayerCharacter.Strength -= Bonus;
          break;
        case ("Dexterity"):
          _globalPlayer.PlayerCharacter.Dexterity -= Bonus;
          break;
        case ("Vitality"):
          _globalPlayer.PlayerCharacter.Vitality -= Bonus;
          break;
        case ("Intelligence"):
          _globalPlayer.PlayerCharacter.Intelligence -= Bonus;
          break;
        case ("Luck"):
          _globalPlayer.PlayerCharacter.Luck -= Bonus;
          break;
      }
      if (Type.Equals("Weapon")) {
        _globalPlayer.Inventory.Add(_globalPlayer.EquippedWeapon);
        _globalPlayer.EquippedWeapon = null;
      }
      else
      {
        _globalPlayer.EquippedArmor = null;
        _globalPlayer.Inventory.Add(this);
      }
    }
  }
}
