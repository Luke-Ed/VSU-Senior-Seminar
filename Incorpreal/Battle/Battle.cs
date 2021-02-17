using Godot;
using System;

public class Battle : Node
{
    public Vector2 playerPosition;


    public Battle()
    {

    }

    //This is just for demo expamples to not get stuck on battle screen.
    public void _on_Button_pressed()
    {
        GetTree().ChangeScene("res://Game.tscn");
    }

}
