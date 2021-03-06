using Godot;
using System;

public class LevelUp : Node
{
    public int stat;
    public OptionButton ob;

    public override void _Ready()
    {
        ob = GetNode<OptionButton>("OptionButton");
    }

    public void _on_Button_pressed()
    {
        stat = ob.Selected;
        Console.WriteLine(stat);
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        gp.LevelUp(stat);
        GetTree().ChangeScene(gp.lastScene);
    }

}
