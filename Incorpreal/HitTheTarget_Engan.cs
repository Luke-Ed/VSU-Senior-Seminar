using Godot;
using System;
using System.Collections.Generic;

public class HitTheTarget_Engan : Node
{

    public ColorRect targetPage;
    public ColorRect battlePage;
    public KinematicBody2D player;
    public int numberOfTargets;
    public List<StaticBody2D> targets = new List<StaticBody2D>();


    public override void _Ready()
    {
        targetPage = GetNode<ColorRect>("HitTheTarget");
        player = targetPage.GetNode<KinematicBody2D>("Player");
        for (int i = 0; i < 4; i++)
        {
            String s = "Target" + (i + 1);
            targets.Add(targetPage.GetNode<StaticBody2D>(s));
        }
        numberOfTargets = 4;
    }

    public void minigameStart()
    {
        showTargets();
        numberOfTargets = 4;
    }

    public void showTargets()
    {
        foreach(StaticBody2D target in targets)
        {
            target.Visible = true;
        }
    }

    public void _on_Target_tree_exited()
    {
        Console.WriteLine("got hit");
        numberOfTargets--;
        if (numberOfTargets == 0)
        {
            Console.WriteLine("you win");
        }
    }

}
