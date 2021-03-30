using Godot;
using System;

public class TimingGame : Node2D
{

    private Boolean _isGood = false;
    private Boolean _isPerfect = false;
    private Timer _timer;
    private ColorRect _gamePage; 

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Connect("timeout", this, "onTimeout");
        _timer.WaitTime = 10;
        _gamePage = GetNode<ColorRect>("GamePage");
    }


    public void _on_Good_body_entered(Node body)
    {
        _isGood = true;
        Console.WriteLine("good");
    }

    public void _on_Good_body_exited(Node body)
    {
        _isGood = false;
        Console.WriteLine("not good");
    }


    public void _on_Perfect_body_entered(Node body)
    {
        _isPerfect = true;
        Console.WriteLine("perfect");
    }

    public void _on_Perfect_body_exited(Node body)
    {
        _isPerfect = false;
        Console.WriteLine("not perfect");
    }

    public void onTimeout()
    {
        _gamePage.Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left_click"))
        {
            if (_isGood)
            {
                if (_isPerfect)
                {

                }
            }
        }
    }
}



