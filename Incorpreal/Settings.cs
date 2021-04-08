using Godot;
using System;

namespace Incorpreal
{
    public class Settings : Control
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        public HSlider SoundSlider;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SoundSlider = (HSlider)GetNode("HSlider");
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}