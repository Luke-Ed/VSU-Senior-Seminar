using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bat : AbstractEnemy {
    public float time=0;
    
    private GlobalPlayer _globalPlayer;
    private KinematicBody2D _player;
    public AudioStreamPlayer2D footsteps = new AudioStreamPlayer2D();
    
    public Bat() : 
      base(150, 2, 30, "Bat") {
    }

    public override void _PhysicsProcess(float delta) {
      if (_player != null) {
        Vector2 velocity = GlobalPosition.DirectionTo(_player.GlobalPosition);
        MoveAndCollide(velocity * MoveSpeed * delta);
      }
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      this.AddChild(footsteps);
        const string Path = "res://sounds/BatSound.wav";
        AudioStream footstep = (AudioStream)GD.Load(Path);
        footsteps.Stream = footstep;
        footsteps.Autoplay = true;
        footsteps.MaxDistance = 300;
        footsteps.Attenuation = 3;
        footsteps.VolumeDb = (1);
    }

    public Boolean playTurn() {
      TurnQueue turnQueue = (TurnQueue)GetNode("/root/Tq");
      Boolean didHit;
      if (!_globalPlayer.isDefending) {
        didHit = _globalPlayer.takeDamage(Attack);
      }
      else{
        if (_globalPlayer.didBlock){
          didHit = _globalPlayer.takeDamage(0);
        }
        else{
          didHit = _globalPlayer.takeDamage(Attack / 2);
        }
      }
      if (didHit){
        switch (EnemyType){
          case ("Bat"):
            if (turnQueue.EnemyCurrentHp + Attack <= turnQueue.EnemyMaxHp){
              turnQueue.EnemyCurrentHp += Attack;
            }
            break;
          case ("Bear"):
            _globalPlayer.Status = "Bleeding";
            break;
          case ("Goblin"):
            //Need something for goblin
            break;
          case ("Skeleton"):
            //Need something for skeleton
            break;
          case ("Snake"):
            //Need something for snake
            break;
          case ("Wolf"):
            //Need something for wolf
            break;
          default:
            break;
          }
        }
        return didHit;
    }


    public void Hit() { 
      if (!_globalPlayer.isPossesing) { 
        //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        _globalPlayer.enemiesFought.Add(Name);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).Name = tq.EnemyName;
        tq.GetChild(1).Call("_Ready");
        GetTree().ChangeScene("res://Battle.tscn");
      }
    }

    public void _on_Area2D_body_entered(Node body) {
      if (body.Name == "Player") {
        //player = (KinematicBody2D)body;
      }
    }

    public void _on_Area2D_body_exited(Node body) {
      if (body.Name == "Player") {
        _player = null;
      }
    }
    public override void _Process(float delta){
      time += delta;
      if (footsteps.Playing == false) {
        if (time > 5) {
          time = 0;
          footsteps.Play();
          }
      }
    }

  }
}
