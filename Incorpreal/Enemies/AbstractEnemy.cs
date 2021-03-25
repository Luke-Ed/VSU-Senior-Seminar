using Godot;

namespace Incorpreal.Enemies {
  public abstract class AbstractEnemy : KinematicBody2D {
    protected int MoveSpeed { get; }
    protected int Attack { get; }
    protected int Health { get; }
    protected int CurrentHealth { get; }
    protected string EnemyName { get; }
    private GlobalPlayer _globalPlayer;
    private KinematicBody2D _player;

    protected AbstractEnemy(int moveSpeed, int attack, int health, string enemyName) {
      MoveSpeed = moveSpeed;
      Attack = attack;
      Health = health;
      CurrentHealth = Health;
      EnemyName = enemyName;
    }

    public override void _Ready() {
        
    }
  
  }
}
