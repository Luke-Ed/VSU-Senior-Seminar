using Godot;

namespace Incorpreal.Enemies {
  public abstract class AbstractEnemy : KinematicBody2D {
    protected int MoveSpeed { get; }
    protected int Attack { get; }
    protected int Health { get; }
    protected int CurrentHealth { get; }
    protected string EnemyType { get; }

    protected AbstractEnemy(int moveSpeed, int attack, int health, string enemyType) {
      MoveSpeed = moveSpeed;
      Attack = attack;
      Health = health;
      CurrentHealth = Health;
      EnemyType = enemyType;
    }

    public override void _Ready() {
        
    }
  
  }
}
