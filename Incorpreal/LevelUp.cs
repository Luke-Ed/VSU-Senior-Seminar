using System;
using Godot;

namespace Incorpreal {
  public class LevelUp : Node {
    private int _stat;
    private Label _lblStr;
    private Label _lblDex;
    private Label _lblVit;
    private Label _lblInt;
    private Label _lblLuck;
    private RichTextLabel _statInfo;

    public override void _Ready() {
      _lblStr = (Label)GetNode("Strength");
      _lblDex = (Label)GetNode("Dexterity");
      _lblVit = (Label)GetNode("Vitality");
      _lblInt = (Label)GetNode("Intelligence");
      _lblLuck = (Label)GetNode("Luck");
      _statInfo = (RichTextLabel)GetNode("Info");
      _statInfo.Text = "Hover over a stat for more information";
    }

    public void _on_Str_mouse_entered() {
      _lblStr.AddColorOverride("font_color", Colors.Red);
      _statInfo.Text = "Increase Melee Damage.";
      _stat = 1;
      Console.WriteLine("Hover str");
    }

    public void _on_Dex_mouse_entered() {
      _lblDex.AddColorOverride("font_color", Colors.Red);
      _statInfo.Text = "Increase Range Damage.";
      _stat = 2;
    }

    public void _on_Vit_mouse_entered() {
      _lblVit.AddColorOverride("font_color", Colors.Red);
      _statInfo.Text = "Increase Health.";
      _stat = 3;
    }

    public void _on_Int_mouse_entered() {
      _lblInt.AddColorOverride("font_color", Colors.Red);
      _statInfo.Text = "Increase Chance To Hit/Dodge and Increase Spell Damage.";
      _stat = 4;
    }

    public void _on_Luck_mouse_entered() {
      _lblLuck.AddColorOverride("font_color", Colors.Red);
      _statInfo.Text = "Increase Critical Hit Chance.";
      _stat = 5;
    }

    public void _on_Str_mouse_exited() {
      _lblStr.AddColorOverride("font_color", Colors.White);
      _statInfo.Text = "Hover over a stat for more information";
      _stat = 0;
    }

    public void _on_Dex_mouse_exited()
    {
      _lblDex.AddColorOverride("font_color", Colors.White);
      _statInfo.Text = "Hover over a stat for more information";
      _stat = 0;
    }

    public void _on_Vit_mouse_exited() {
      _lblVit.AddColorOverride("font_color", Colors.White);
      _statInfo.Text = "Hover over a stat for more information";
      _stat = 0;
    }

    public void _on_Int_mouse_exited() {
      _lblInt.AddColorOverride("font_color", Colors.White);
      _statInfo.Text = "Hover over a stat for more information";
      _stat = 0;
    }

    public void _on_Luck_mouse_exited() {
      _lblLuck.AddColorOverride("font_color", Colors.White);
      _statInfo.Text = "Hover over a stat for more information";
      _stat = 0;
    }

    public void _on_Select_gui_input(InputEvent @event) {
      if (_stat != 0 && @event is InputEventMouseButton) {
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        gp.LevelUp(_stat);
        GetTree().ChangeScene(gp.lastScene);
      }   
    }
  }
}
