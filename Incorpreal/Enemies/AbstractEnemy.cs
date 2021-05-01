using System;
using System.ComponentModel.Design;
using Godot;

namespace Incorpreal.Enemies {
  public abstract class AbstractEnemy : KinematicBody2D {
    protected int MoveSpeed { get; }
    protected int Attack { get; }
    protected int Health { get; }
    protected int CurrentHealth { get; set; }
    protected string EnemyType { get; }
    protected string StatusEffect { get; }

    public KinematicBody2D Player;
    protected GlobalPlayer GlobalPlayer;
    protected Timer Timer;
    private Boolean _battleStarting;
    private Navigation2D _navigation;
    private Vector2 _startingPos;
    private readonly Random _random = new Random();
    public Boolean OnCamera { get; set; }
    protected AbstractEnemy(int moveSpeed, int attack, int health, string enemyType, string statusEffect) {
      MoveSpeed = moveSpeed;
      Attack = attack;
      Health = health;
      CurrentHealth = Health;
      EnemyType = enemyType;
      StatusEffect = statusEffect;
    }

    public override void _Ready() {
      GlobalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      _startingPos = Position;
      if (HasNode("Timer")) {
        Timer = GetNode<Timer>("Timer");
        Timer.Connect("timeout", this, "onTimeout");
        Timer.WaitTime = 5;
      }
    }
    
    public override void _PhysicsProcess(float delta) {
      try {
        Vector2 direction = (Player.GlobalPosition - GlobalPosition).Normalized();
        MoveAndCollide(direction * MoveSpeed * delta);
      }
      catch {
        //throw new NotImplementedException();
      }
    }
    
    public void _on_Area2D_body_entered(Node body) {
      if (body.Name == "Player") {
        Player = (KinematicBody2D)body;
        AddToGroup("Following");
      }
    }

    public void _on_Area2D_body_exited(Node body) {
      if (body.Name == "Player") {
        Player = null;
        if (!_battleStarting) {
          Timer.Start();
        }
      }
    }
    
    //After the player gets out of range of the enemy and will continue to stay in that spot after 10 seconds if the enemy is no longer on screen then it is returned to the original starting position
    //and follows the original path if it has one by removing it from the "Following" group. If the enemy is still on screen the timer refreshes and will check again in another 10 seconds.
    public void onTimeout() {
      if (OnCamera) {
        Timer.Start();
      }
      else {
        Position = _startingPos;
        RemoveFromGroup("Following");
      }
    }
    
    public void Hit() { 
      if (!GlobalPlayer.isPossesing) { 
        //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
        Timer.Stop();
        _battleStarting = true;
        GlobalPlayer.EnemiesFought.Add(Name);
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        tq.GetChild(1).SetScript(GetScript());
        tq.GetChild(1).Name = EnemyType;
        tq.GetChild(1).Call("_Ready");
        GetTree().ChangeScene("res://Battle.tscn");
      }
    }
    
    public Boolean PlayTurn() {
      TurnQueue turnQueue = (TurnQueue)GetNode("/root/Tq");
      Boolean didHit;
      if (!GlobalPlayer.isDefending) {
        didHit = GlobalPlayer.takeDamage(Attack);
      }
      else{
        if (GlobalPlayer.didBlock){
          didHit = GlobalPlayer.takeDamage(0);
        }
        else{
          didHit = GlobalPlayer.takeDamage(Attack / 2);
        }
      }
      if (didHit && _random.Next(0,10) > 3){
        // Thirty percent chance to apply a status effect if the enemy was hit.
        ApplyStatusEffect();
      }
      return didHit;
    }
    protected abstract void ApplyStatusEffect();
    // Allows the method to be overridden in relevant enemy classes, applying the status effect. 
  }
}
