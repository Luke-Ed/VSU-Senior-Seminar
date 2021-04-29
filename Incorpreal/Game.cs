using Godot;
using System;

public class Game : Node
{
	public Game()
	{

	}

	public AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
	const string Path = "res://sounds/test.wav";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddChild(audioStreamPlayer);
		AudioStream Background = (AudioStream)GD.Load(Path);
		audioStreamPlayer.Stream = Background;
		audioStreamPlayer.Play();
		audioStreamPlayer.VolumeDb = (-20);
		audioStreamPlayer.PauseMode = Node.PauseModeEnum.Process; //This line keeps music playing during pause
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
