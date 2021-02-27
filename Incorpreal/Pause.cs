using Godot;
using System;

public class Pause : Control
{
    public Label PauseLabel;
    public Label QuitLabel;
    public Boolean quitEntered = false;

    //Pause listener
    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("pause")) {
            PauseGame();
        }
    }

    public void PauseGame() {
        var pauseBoolean = !GetTree().Paused; //Reverse current pause status
        GetTree().Paused = pauseBoolean; //Set it
        this.Visible = pauseBoolean; //Make pause screen appear/dissappear
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PauseLabel = (Label)GetNode("PauseLabel"); //Grab pause label
        QuitLabel = (Label)GetNode("QuitLabel"); //Grab quit label
    }

    public void _on_QuitLabel_mouse_entered() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkRed);
        quitEntered = true;
    }

    public void _on_QuitLabel_mouse_exited() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkGray);
        quitEntered = false;
    }

    public void _on_QuitLabel_gui_input(InputEvent @event) {
        if (quitEntered && @event is InputEventMouseButton) {
            GetTree().Quit();
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

}
