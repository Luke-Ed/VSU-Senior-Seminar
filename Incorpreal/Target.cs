using Godot;

public class Target : StaticBody2D {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";
  public AudioStreamPlayer hit = new AudioStreamPlayer();
  AudioStream footstep = (AudioStream)GD.Load("res://sounds/SkeletonHitSound.wav");
  // Called when the node enters the scene tree for the first time.
  public override void _Ready(){
    this.AddChild(hit);
    hit.Stream=footstep;
  }

  public void Hit() {
    Visible = false;
    hit.Play();
  }
}

