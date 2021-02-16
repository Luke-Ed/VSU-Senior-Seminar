using Godot;
using System;

public class LevelScript : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void SpawnEnemy(string resPath, Vector2 position, Node currentScene) {
        PackedScene enemyScene = (PackedScene) ResourceLoader.Load(resPath); //Load resource
        KinematicBody2D enemy = (KinematicBody2D)enemyScene.Instance(); //Instantiate
        currentScene.AddChild(enemy); //Add to scene
        enemy.GlobalPosition = position; //Set to original position
        //enemy.ChangeState("confused"); //Doesn't exist yet
    }
    
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
