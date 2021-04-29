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

    protected AbstractEnemy(int moveSpeed, int attack, int health, string enemyType, string statusEffect) {
      MoveSpeed = moveSpeed;
      Attack = attack;
      Health = health;
      CurrentHealth = Health;
      EnemyType = enemyType;
      StatusEffect = statusEffect;
    }

    public override void _Ready() {
        
    }

    protected abstract void ApplyStatusEffect();
    // Allows the method to be overridden in relevant enemy classes, applying the status effect. 
  }
}
