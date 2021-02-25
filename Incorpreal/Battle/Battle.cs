using Godot;
using System;

public class Battle : Node
{


    public Battle()
    {

    }
    public AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
    AudioStream HammerAttack = (AudioStream)GD.Load("res://sounds/HammerClank.wav");
    AudioStream BattleStart = (AudioStream)GD.Load("res://sounds/scaletest.wav");
    public override void _Ready()
    {
        this.AddChild(audioStreamPlayer);
        audioStreamPlayer.Stream = BattleStart;
        audioStreamPlayer.Play();
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        var healthLabel = GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

    //This is just for demo expamples to not get stuck on battle screen.
    public void _on_Button_pressed()
    {
       GetTree().ChangeScene("res://Game.tscn");
    }

    public void _on_TakeDamageButton_pressed()
    {
        audioStreamPlayer.Stream = HammerAttack;
        audioStreamPlayer.Play();
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        gp.takeDamage(5);
        var healthLabel = GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

}
