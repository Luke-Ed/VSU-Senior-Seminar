using Godot;
using System;

public class Pause : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

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
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

}
