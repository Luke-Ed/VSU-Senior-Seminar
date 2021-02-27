using Godot;
using System;

public class Game : Node
{

    public Game()
    {
    }


	// Declare member variables here. Examples:
	// private int a = 2;
	public AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
	// private string b = "text";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddChild(audioStreamPlayer);
		AudioStream Background = (AudioStream)GD.Load("res://sounds/test.wav");
		audioStreamPlayer.Stream = Background;
		audioStreamPlayer.Play();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
