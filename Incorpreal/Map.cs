using Godot;
using System;

public class Map : Node2D
{
    public Vector2 playerPostion;
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
}
