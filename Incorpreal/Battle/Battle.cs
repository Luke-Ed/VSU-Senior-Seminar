using Godot;
using System;

public class Battle : Node {

  //This is just for demo expamples to not get stuck on battle screen.
	public void _on_Button_pressed() {
		GetTree().ChangeScene("res://Level 1.tscn");
	}

}
