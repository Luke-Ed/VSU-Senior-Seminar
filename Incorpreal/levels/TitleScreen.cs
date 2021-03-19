using Godot;
using System;

public class TitleScreen : Control
{
    public Label StartLabel;
    public Label QuitLabel;
    public Boolean StartEntered = false;
    public Boolean QuitEntered = false;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartLabel = (Label)GetNode("StartLabel"); //Grab pause label
        QuitLabel = (Label)GetNode("QuitLabel"); //Grab quit label
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
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
