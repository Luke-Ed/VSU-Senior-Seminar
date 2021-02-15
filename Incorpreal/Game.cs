using Godot;
using System;

public class Game : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	AudioStreamPlayer audioStreamPlayer = audioStreamPlayer.new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self.add_child(audioStreamPlayer);
		audioStreamPlayer.stream = GD.load("res://Sounds/test.wav");
		audioStreamPlayer.set_volume_db(-10);
		audioStreamPlayer.play();
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
