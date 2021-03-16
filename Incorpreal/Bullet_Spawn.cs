using Godot;
using System;

public class Bullet_Spawn : Node2D
{

    Resource bhScene = ResourceLoader.Load("res://BulletHell_Engan.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        Rotation = 5;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
