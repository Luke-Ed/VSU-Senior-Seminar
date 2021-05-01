using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bat : AbstractEnemy {
    
    private GlobalPlayer _globalPlayer;
    public KinematicBody2D player;
    private Navigation2D _navigation;
    private Vector2 _startingPos;
    private Timer _timer;
    private Boolean _battleStarting;
    public Boolean OnCamera { get; set; }
    



    private readonly Random _random = new Random();
    public Bat() : 
      base(50, 2, 30, "Bat", "Leeching") {
    }

    public override void _PhysicsProcess(float delta) {
      try {
        Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
        MoveAndCollide(direction * MoveSpeed * delta);
      }
      catch {
        //throw new NotImplementedException();
      }
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      _startingPos = Position;
      if (HasNode("Timer")) {
        _timer = GetNode<Timer>("Timer");
        _timer.Connect("timeout", this, "onTimeout");
        _timer.WaitTime = 5;
      }
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
      if (!_globalPlayer.isPossesing) { 
        //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        _timer.Stop();
        _battleStarting = true;
        _globalPlayer.EnemiesFought.Add(Name);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).Name = EnemyType;
        tq.GetChild(1).Call("_Ready");
        GetTree().ChangeScene("res://Battle.tscn");
      }
    }

    public void _on_Area2D_body_entered(Node body) {
      if (body.Name == "Player") {
        player = (KinematicBody2D)body;
        AddToGroup("Following");
      }
    }

    public void _on_Area2D_body_exited(Node body) {
      if (body.Name == "Player") {
        player = null;
        if (!_battleStarting) {
          _timer.Start();
        }
      }
    }

    //After the player gets out of range of the enemy and will continue to stay in that spot after 10 seconds if the enemy is no longer on screen then it is returned to the original starting position
    //and follows the original path if it has one by removing it from the "Following" group. If the enemy is still on screen the timer refreshes and will check again in another 10 seconds.
    public void onTimeout() {
      if (OnCamera) {
        _timer.Start();
      }
      else {
        Position = _startingPos;
        RemoveFromGroup("Following");
      }
    }
  }
}
