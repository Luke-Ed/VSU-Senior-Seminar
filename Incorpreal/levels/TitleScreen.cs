using Godot;
using System;

public class TitleScreen : Control
{
    public Label StartLabel;
    public Label QuitLabel;
    public Label SettingLabel;
    public Boolean StartEntered = false;
    public Boolean QuitEntered = false;
    public Boolean SettingEntered = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartLabel = (Label)GetNode("StartLabel"); //Grab pause label
        QuitLabel = (Label)GetNode("QuitLabel");//Grab quit label
        SettingLabel = (Label)GetNode("SettingLabel");
    }

    public void _on_StartLabel_mouse_entered() {
        StartLabel.AddColorOverride("font_color", Colors.DarkRed);
        StartEntered = true;
    }

    public void _on_StartLabel_mouse_exited() {
        StartLabel.AddColorOverride("font_color", Colors.DarkGray);
        StartEntered = false;
    }

    public void _on_QuitLabel_mouse_entered() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkRed);
        QuitEntered = true;
    }

    public void _on_QuitLabel_mouse_exited() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkGray);
        QuitEntered = false;
    }

    public void _on_QuitLabel_gui_input(InputEvent @event) {
        if (QuitEntered && @event is InputEventMouseButton) {
            GetTree().Quit();
        }
    }

    public void _on_SettingLabel_mouse_entered()
    {
       SettingLabel.AddColorOverride("font_color", Colors.DarkRed);
        SettingEntered = true;
    }

    public void _on_SettingLabel_mouse_exited()
    {
        SettingLabel.AddColorOverride("font_color", Colors.DarkGray);
        SettingEntered = false;
    }

    public void _on_StartLabel_gui_input(InputEvent @event) {
        if (StartEntered && @event is InputEventMouseButton) {
            GetTree().ChangeScene("res://Level 1.tscn"); //Need to remake level1 first
        }
    }
    public void _on_SettingLabel_gui_input(InputEvent @event)
    {
        if (SettingEntered && @event is InputEventMouseButton)
        {
            GetTree().ChangeScene("res://Level 1.tscn"); //Need to remake level1 first
        }
    }
}
