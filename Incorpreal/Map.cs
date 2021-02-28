using Godot;
using System;

public class Map : Node2D
{
    public Vector2 playerPostion;
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
    }
}
