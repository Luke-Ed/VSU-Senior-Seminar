using Godot;
using System;

//Trying to keep this worded so that it works with all levels, not just one
public class LevelScript : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    /* This method spawns an enemy into the given scene at the given position
        resPath - A string pointing to the resource URL. Ex: "res://Enemies/Bat.tscn" for the bat. You can right click a scene in Godot and "Copy Path" for this
        position - A Vector2 to serve as spawn location. Can be accessed like this: Vector2 originalPlayerPos = this.GlobalPosition;
        currentScene - The scene which SpawnEnemy() was called from. Can be accessed like this: GetTree().CurrentScene
    */
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
