using Godot;
using System;

public class Battle : Node
{


    public Battle()
    {

    }

    public override void _Ready()
    {
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        var healthLabel = GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

    //This is just for demo expamples to not get stuck on battle screen.
    public void _on_Button_pressed()
    {
       GetTree().ChangeScene("res://Level 1.tscn");
    }

    public void _on_TakeDamageButton_pressed()
    {
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        gp.takeDamage(5);
        var healthLabel = GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

}
