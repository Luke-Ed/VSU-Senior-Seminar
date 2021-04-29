using Godot;
using System;

public class Game : Node {
  public Game() {
  }
    
	public AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
	const string Path = "res://sounds/test.wav";
	
	public override void _Ready() {
	  AddChild(audioStreamPlayer);
		AudioStream Background = (AudioStream)GD.Load(Path);
		audioStreamPlayer.Stream = Background;
		audioStreamPlayer.Play();
		audioStreamPlayer.VolumeDb = -20;
		audioStreamPlayer.PauseMode = PauseModeEnum.Process; 
    //This line keeps music playing during pause
	}
  
}
