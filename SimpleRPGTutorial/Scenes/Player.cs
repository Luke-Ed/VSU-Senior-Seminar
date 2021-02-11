using Godot;
using System;

public class Player : KinematicBody2D
{
 [Export] public int speed = 70;

 public Vector2 velocity;
 
 // Get player input
 public void GetInput()
 {
	 Godot.Sprite child = this.GetNode<Godot.Sprite>("Sprite");
		velocity = new Vector2();

		if (Input.IsActionPressed("ui_right")) {
			velocity.x += 1;
		}

		if (Input.IsActionPressed("ui_left")) {
			velocity.x -= 1;
		}

		if (Input.IsActionPressed("ui_down")) {
			velocity.y += 1;
		}

		if (Input.IsActionPressed("ui_up")) {
			velocity.y -= 1;
		}

		velocity = velocity.Normalized() * speed;
 }

//Process movement and collsion
 public override void _PhysicsProcess(float delta) {
	 GetInput();
	 velocity = MoveAndSlide(velocity);
 }
}
