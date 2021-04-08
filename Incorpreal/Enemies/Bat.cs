using System;
using Godot;
using Incorpreal.Enemies;

namespace Incorpreal.bat {
  public class Bat : AbstractEnemy {
    
    public GlobalPlayer _globalPlayer;
    public KinematicBody2D player;

    public Bat() : 
      base(50, 2, 30, "Bat") {
    }

    public override void _PhysicsProcess(float delta) {
      
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    public Boolean PlayTurn() {
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
      if (!_globalPlayer.isPossesing) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        _globalPlayer.enemiesFought.Add(Name);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).Name = EnemyType;
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
        player = null;
      }
    }

  }
}
