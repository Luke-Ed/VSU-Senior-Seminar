using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bear : AbstractEnemy {
    private GlobalPlayer _globalPlayer;
    private KinematicBody2D _player;

    public Bear() :
      base(150, 5, 60, "Bear", "Bleeding") {
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer) GetNode("/root/GlobalData");
    }

    protected override void ApplyStatusEffect() {
      if (!_globalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        _globalPlayer.PlayerCharacter.StatusEffect = "Bleeding";
      }
    }

    public override void _PhysicsProcess(float delta) {
      if (_player != null) {
        Vector2 velocity = GlobalPosition.DirectionTo(_player.GlobalPosition);
        MoveAndCollide(velocity * MoveSpeed * delta);
      }
    }

    public void Hit() {
      if (!_globalPlayer.isPossesing) {
        _globalPlayer.EnemiesFought.Add(Name);
        TurnQueue turnQueue = (TurnQueue) GetNode("/root/Tq");
        turnQueue.GetChild(1).Name = EnemyType;
        turnQueue.GetChild(1).Call("_Ready");
        GetTree().ChangeScene("res://Battle.tscn");
      }
    }


    public Boolean PlayTurn() {
      if (!_globalPlayer.isDefending) {
        return _globalPlayer.takeDamage(Attack);
      }
      if (_globalPlayer.didBlock) {
        return _globalPlayer.takeDamage(0);
      }
      else {
        return _globalPlayer.takeDamage(Attack / 2);
      }
    }
  }
}
