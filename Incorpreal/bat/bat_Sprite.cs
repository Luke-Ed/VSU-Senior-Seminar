using Godot;
using System;

public class bat_Sprite : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public float time = 0;
    public AudioStreamPlayer2D footsteps = new AudioStreamPlayer2D();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddChild(footsteps);
        const string Path = "res://sounds/BatSound.wav";
        AudioStream footstep = (AudioStream)GD.Load(Path);
        footsteps.Stream = footstep;
        footsteps.Autoplay = true;
        footsteps.MaxDistance = 300;
        footsteps.Attenuation = 6;
        footsteps.VolumeDb = (1);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
 {
              time += delta;
        if (footsteps.Playing == false)
        {
            if (time > 2.5)
            {
                time = 0;
                footsteps.Play();
            }
        }
 }
}
