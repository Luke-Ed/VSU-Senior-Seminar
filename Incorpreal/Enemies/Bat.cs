using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bat : AbstractEnemy {
    
    public GlobalPlayer _globalPlayer;
    public KinematicBody2D player;
    private Random _random;
    public Bat() : 
      base(50, 2, 30, "Bat", "Leeching") {
    }

    public override void _PhysicsProcess(float delta) {
      
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    protected override void ApplyStatusEffect() {
      if(CurrentHealth + Attack <= Health) {
        CurrentHealth += Attack;
      }
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
      if (didHit && _random.Next(0,10) > 3){
        // Thirty percent chance to apply a status effect if the enemy was hit.
        ApplyStatusEffect();
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
