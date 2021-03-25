using System;
using Godot;
using Incorpreal.Enemies;

namespace Incorpreal.bat {
  public class Bat : AbstractEnemy {
    
    public GlobalPlayer gp;
    public KinematicBody2D player;
    
    public Bat() : 
      base(150, 2, 30, "Bat") {
    }

    public override void _PhysicsProcess(float delta) {
      if (player != null) {
        Vector2 velocity = GlobalPosition.DirectionTo(player.GlobalPosition);
        MoveAndCollide(velocity * MoveSpeed * delta);
      }
    }

    public override void _Ready() {
      gp = (GlobalPlayer)GetNode("/root/GlobalData");
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

    public Boolean playTurn() {
      if (!gp.isDefending) {
        return gp.takeDamage(Attack);
      }
      else
      {
        if (gp.didBlock)
        {
          return gp.takeDamage(0);
        }
        else
        {
          return gp.takeDamage(Attack / 2);
        }
      }
    }


    public void Hit() { 
      if (!gp.isPossesing) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        gp.enemyFought.Add(Name);
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
        player = null;
      }
    }

  }
}
