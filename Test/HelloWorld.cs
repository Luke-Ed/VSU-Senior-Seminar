using Godot;
using System;

public class HelloWorld : Panel
{
	// Declare member variables here.
	private Label mainLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mainLabel = GetNode<Label>("mainLabel");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	private void _on_Button_pressed()
	{
		mainLabel.Text = "Hello World";
	}
}
