using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bat : AbstractEnemy {
    
    private GlobalPlayer _globalPlayer;
    private KinematicBody2D _player;
    
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
      /*
      var filePath = "res://Enemies/enemies.txt";
      File newFile = new File();
      newFile.Open(filePath, File.ModeFlags.Read);
      
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
      */
    }

    public Boolean PlayTurn() {
      if (!_globalPlayer.isDefending) {
        return _globalPlayer.takeDamage(Attack);
      }
      else
      {
        if (_globalPlayer.didBlock)
        {
          return _globalPlayer.takeDamage(0);
        }
        else
        {
          return _globalPlayer.takeDamage(Attack / 2);
        }
      }
    }


    public void Hit() { 
      if (!_globalPlayer.isPossesing) { 
        //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        _globalPlayer.enemyFought.Add(Name);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).Name = EnemyName;
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

  }
}
