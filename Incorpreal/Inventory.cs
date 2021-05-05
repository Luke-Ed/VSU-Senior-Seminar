using System.Collections.Generic;
using Godot;

namespace Incorpreal {
  public class Inventory : Control {
    private RichTextLabel _statText;
    private GridContainer _invMenu;
    private GlobalPlayer _globalPlayer;
    private Control _pauseMenu;

    public override void _Ready() {
      _pauseMenu = (Control)GetNode("../../PauseMenu/Pause");
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      _statText = (RichTextLabel)GetNode("TextureRect").GetNode("StatText");
      _invMenu = (GridContainer)GetNode("TextureRect").GetNode("GridContainer");
      //Is ran everytime it is loaded in order to refill the inventory and equiped items based on what is in the global player.
      //Had to do a work around by assinging the new item created as the ones held in globalplayer due to items being duplicated because of the items being created in the inventory menu are not
      //the same ones within the globalplayer, so could not add and remove them.
      if (_globalPlayer.EquippedArmor != null) {
        PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
        Item tempItem = (Item)ItemScene.Instance();
        tempItem.changePicture(_globalPlayer.EquippedArmor.SpritePath);
        tempItem.GiveProperties(_globalPlayer.EquippedArmor.ItemName, _globalPlayer.EquippedArmor.Type, _globalPlayer.EquippedArmor.Stat, _globalPlayer.EquippedArmor.Bonus);
        _globalPlayer.EquippedArmor = tempItem;
        FindNode("EquipedArmor").AddChild(_globalPlayer.EquippedArmor);
        FindNode("EquipedArmor").Set("item", _globalPlayer.EquippedArmor);
      }
      if (_globalPlayer.EquippedWeapon != null) {
        PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
        Item tempItem = (Item)ItemScene.Instance();
        tempItem.changePicture(_globalPlayer.EquippedWeapon.SpritePath);
        tempItem.GiveProperties(_globalPlayer.EquippedWeapon.ItemName, _globalPlayer.EquippedWeapon.Type, _globalPlayer.EquippedWeapon.Stat, _globalPlayer.EquippedWeapon.Bonus);
        _globalPlayer.EquippedWeapon = tempItem;
        FindNode("EquipedWeapon").AddChild(tempItem);
        FindNode("EquipedWeapon").Set("item", tempItem);
      }
      if (_globalPlayer.Inventory.Count > 0) {
        List<Item> newItems = new List<Item>();
        foreach (Item item in _globalPlayer.Inventory) {
          PackedScene ItemScene = (PackedScene)ResourceLoader.Load("res://Item.tscn");
          Item tempItem = (Item)ItemScene.Instance();
          tempItem.changePicture(item.SpritePath);
          tempItem.GiveProperties(item.ItemName, item.Type, item.Stat, item.Bonus);
          fillSlot(tempItem);
          newItems.Add(tempItem);
        }
        _globalPlayer.Inventory = newItems;
      }
    }

    public override void _Input(InputEvent @event) {
      if (Input.IsActionJustPressed("inventory") && !_pauseMenu.Visible) {
        openInventory();
      }
    }

    public void fillSlot(Item item) {
      //Goes through each node in the inventory menu, finds an empty one and fills it with the given item.
      foreach (Node slot in _invMenu.GetChildren()) {
        if (slot.Get("item") == null) {
          slot.Set("item", item);
          slot.AddChild(item);
          break;
        }
      }
    }


    //Similar to the pause menu pressing I will bring up the inventory and pause the surrounding game. Pressing I again will undo it.
    private void openInventory() {
      sortInventory();
      Visible = !Visible;
      GetTree().Paused = !GetTree().Paused;
    }

    //Whenever inventory is opened it is sorted so that all your weapons are first then your armors and lastly your consumables.
    private void sortInventory() {
      List<Item> weapons = new List<Item>();
      List<Item> armors = new List<Item>();
      List<Item> consumables = new List<Item>();
      foreach (Node slot in _invMenu.GetChildren()) {
        if(slot.Get("item") != null) {
          slot.RemoveChild((Item)slot.Get("item"));
          slot.Set("item", null);
        }
      }
      foreach (Item item in _globalPlayer.Inventory) {
        if (item.Type.Equals("Weapon")) {
          weapons.Add(item);
        }
        else if (item.Type.Equals("Armor")) {
          armors.Add(item);
        }
        else {
          consumables.Add(item);
        }
      }
      foreach (Item item in weapons) {
        fillSlot(item);
      }
      foreach (Item item in armors) {
        fillSlot(item);
      }
      foreach (Item item in consumables) {
        fillSlot(item);
      }
    }


    private void _on_Slot_mouse_exited() {
      //While you are not hovering any item will display the player's stats.
      _statText.Text =  "Your Stats: \n";
      _statText.Text += "Level: " + _globalPlayer.PlayerCharacter.Level;
      _statText.Text += "\nStrength: " + _globalPlayer.PlayerCharacter.Strength;
      _statText.Text += "\nDexterity: " + _globalPlayer.PlayerCharacter.Dexterity;
      _statText.Text += "\nVitality: " + _globalPlayer.PlayerCharacter.Vitality;
      _statText.Text += "\nIntelligence: " + _globalPlayer.PlayerCharacter.Intelligence;
      _statText.Text += "\nLuck: " + _globalPlayer.PlayerCharacter.Luck;
    }
  }
}
