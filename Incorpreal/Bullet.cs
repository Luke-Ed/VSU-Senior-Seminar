using Godot;
using System;

public class Bullet : KinematicBody2D {
  Vector2 _velocity;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    _velocity.y = -600;
  }

  public override void _PhysicsProcess(float delta) {
    if (Position.y < -1000) {
      QueueFree();
    }
    MoveAndSlide(_velocity);

    var collision = MoveAndCollide(_velocity.Normalized() * delta);

    if (collision != null && collision.Collider.HasMethod("Hit")) {
      collision.Collider.Call("Hit");
        QueueFree();
    }
  }
}
