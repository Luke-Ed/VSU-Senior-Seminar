using Godot;
using System;

public class Game2 : Node
{
	public Game2()
	{

	}

	public AudioStreamPlayer background = new AudioStreamPlayer();
	const string Path = "res://sounds/level2.wav";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddChild(background);
		AudioStream Background = (AudioStream)GD.Load(Path);
		background.Stream = Background;
		background.Play();
		background.VolumeDb = (-20);
		background.PauseMode = Node.PauseModeEnum.Process; //This line keeps music playing during pause
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
