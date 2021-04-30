using Godot;
using System;

public class TitleScreen : Control
{
    public Label StartLabel;
    public Label QuitLabel;
    public Label LoadLabel;
    public SaveLoadGame saveLoadGame;
    public Boolean StartEntered = false;
    public Boolean QuitEntered = false;
    public Boolean LoadEntered = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartLabel = (Label)GetNode("StartLabel"); //Grab pause label
        QuitLabel = (Label)GetNode("QuitLabel"); //Grab quit label
        LoadLabel = (Label)GetNode("LoadLabel"); //Grab load label
        saveLoadGame = new SaveLoadGame();
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

    public void _on_LoadLabel_mouse_entered() {
        LoadLabel.AddColorOverride("font_color", Colors.DarkRed);
        LoadEntered = true;
    }

    public void _on_LoadLabel_mouse_exited() {
        LoadLabel.AddColorOverride("font_color", Colors.DarkGray);
        LoadEntered = false;
    }

    public void _on_QuitLabel_gui_input(InputEvent @event) {
        if (QuitEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            GetTree().Quit();
        }
    }

    public void _on_StartLabel_gui_input(InputEvent @event) {
        if (StartEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            GetTree().ChangeScene("res://Level 1.tscn");
        }
    }

    public void _on_LoadLabel_gui_input(InputEvent @event) {
        if (LoadEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            //saveLoadGame.Call("Load"); not added yet
        }
    }
}
