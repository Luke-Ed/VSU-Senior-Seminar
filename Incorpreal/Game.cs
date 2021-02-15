using Godot;
using System;

public class Game : Node
{

    public Game()
    {
    }

    public void startBattle()
    {
        Node2D map = GetNode<Node2D>("res://Game/Map");
        RemoveChild(map);
    }


    public override void _Ready()
    {
        
    }

}
