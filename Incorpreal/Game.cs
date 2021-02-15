using Godot;
using System;

public class Game : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	AudioStreamPlayer audioStreamPlayer= new AudioStreamPlayer();
	const string Path = "res://sounds/test.wav";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer.Stream = ((AudioStream)GD.Load(Path));
		audioStreamPlayer.VolumeDb = (-10);
		audioStreamPlayer.Play();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
