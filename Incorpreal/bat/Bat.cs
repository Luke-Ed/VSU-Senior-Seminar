using Godot;
using System;

public class Bat : KinematicBody2D
{
    [Export]
    public float time=0;
    public int moveSpeed = 150;
    public int attack;
    public int health;
    public int currentHealth;
    public string enemyName;
    public GlobalPlayer gp;
    public KinematicBody2D player;
    public AudioStreamPlayer2D footsteps = new AudioStreamPlayer2D();

    public override void _PhysicsProcess(float delta)
    {
        if (player != null)
        {
            Vector2 velocity = GlobalPosition.DirectionTo(player.GlobalPosition);
            MoveAndCollide(velocity * moveSpeed * delta);
        }
    }

    public override void _Ready()
    {
        this.AddChild(footsteps);
        const string Path = "res://sounds/BatSound.wav";
        AudioStream footstep = (AudioStream)GD.Load(Path);
        footsteps.Stream = footstep;
        footsteps.Autoplay = true;
        footsteps.MaxDistance = 300;
        footsteps.Attenuation = 3;
        footsteps.VolumeDb = (1);
        var filePath = "res://Enemies/enemies.txt";
        File newFile = new File();
        newFile.Open(filePath, File.ModeFlags.Read);
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        while (!newFile.EofReached())
        {
            String s = newFile.GetLine();
            if (Name.Contains(s)){
                enemyName = s;
                attack = int.Parse(newFile.GetLine());
                health = int.Parse(newFile.GetLine());
                currentHealth = health;
            }
        }
    }

    public Boolean playTurn()
    {
        if (!gp.isDefending)
        {
            return gp.takeDamage(attack);
        }
        else
        {
            if (gp.didBlock)
            {
                return gp.takeDamage(0);
            }
            else
            {
                return gp.takeDamage(attack / 2);
            }
        }
    }


    public void Hit()
    { 
        if (!gp.isPossesing) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
            gp.enemyFought.Add(this.Name);
            TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
            tq.GetChild(1).Name = enemyName;
            tq.GetChild(1).Call("_Ready");
            GetTree().ChangeScene("res://Battle.tscn");
            footsteps.Stop();
        }
    }

    public void _on_Area2D_body_entered(Node body)
    {
        if (body.Name == "Player")
        {
            //player = (KinematicBody2D)body;
        }
    }

    public void _on_Area2D_body_exited(Node body)
    {
        if (body.Name == "Player")
        {
            player = null;
        }
    }
    public override void _Process(float delta)
  {
        time += delta;
        if (footsteps.Playing == false)
        {
            if (time > 5)
            {
                time = 0;
                footsteps.Play();
            }
        }

  }

}
