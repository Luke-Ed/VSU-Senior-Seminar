using Godot;
using System;

public class LevelUp : Node
{
    public int stat;
    public Label lblStr;
    public Label lblDex;
    public Label lblVit;
    public Label lblInt;
    public Label lblLuck;
    public RichTextLabel statInfo;

    public override void _Ready()
    {
        lblStr = (Label)GetNode("Strength");
        lblDex = (Label)GetNode("Dexterity");
        lblVit = (Label)GetNode("Vitality");
        lblInt = (Label)GetNode("Intelligence");
        lblLuck = (Label)GetNode("Luck");
        statInfo = (RichTextLabel)GetNode("Info");
        statInfo.Text = "Hover over a stat for more information";
    }

    public void _on_Str_mouse_entered()
    {
        lblStr.AddColorOverride("font_color", Colors.Red);
        statInfo.Text = "Increase Melee Damage.";
        stat = 1;
        Console.WriteLine("Hover str");
    }

    public void _on_Dex_mouse_entered()
    {
        lblDex.AddColorOverride("font_color", Colors.Red);
        statInfo.Text = "Increase Range Damage.";
        stat = 2;
    }

    public void _on_Vit_mouse_entered()
    {
        lblVit.AddColorOverride("font_color", Colors.Red);
        statInfo.Text = "Increase Health.";
        stat = 3;
    }

    public void _on_Int_mouse_entered()
    {
        lblInt.AddColorOverride("font_color", Colors.Red);
        statInfo.Text = "Increase Chance To Hit/Dodge and Increase Spell Damage.";
        stat = 4;
    }

    public void _on_Luck_mouse_entered()
    {
        lblLuck.AddColorOverride("font_color", Colors.Red);
        statInfo.Text = "Increase Critical Hit Chance.";
        stat = 5;
    }

    public void _on_Str_mouse_exited()
    {
        lblStr.AddColorOverride("font_color", Colors.White);
        statInfo.Text = "Hover over a stat for more information";
        stat = 0;
    }

    public void _on_Dex_mouse_exited()
    {
        lblDex.AddColorOverride("font_color", Colors.White);
        statInfo.Text = "Hover over a stat for more information";
        stat = 0;
    }

    public void _on_Vit_mouse_exited()
    {
        lblVit.AddColorOverride("font_color", Colors.White);
        statInfo.Text = "Hover over a stat for more information";
        stat = 0;
    }

    public void _on_Int_mouse_exited()
    {
        lblInt.AddColorOverride("font_color", Colors.White);
        statInfo.Text = "Hover over a stat for more information";
        stat = 0;
    }

    public void _on_Luck_mouse_exited()
    {
        lblLuck.AddColorOverride("font_color", Colors.White);
        statInfo.Text = "Hover over a stat for more information";
        stat = 0;
    }

    public void _on_Select_gui_input(InputEvent @event)
    {
        if (stat != 0 && @event is InputEventMouseButton)
        {
            GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
            gp.LevelUp(stat);
            GetTree().ChangeScene(gp.lastScene);
        }   
    }
}
