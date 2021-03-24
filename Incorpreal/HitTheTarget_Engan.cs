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
    public RichTextLabel rbl;
    public Timer timer;
    public Boolean minigamePlaying = false;
    public Vector2 startingPos;


    public override void _Ready()
    {
        targetPage = GetNode<ColorRect>("HitTheTarget");
        battlePage = GetParent().GetNode<ColorRect>("BattlePage");
        player = targetPage.GetNode<KinematicBody2D>("Player");
        Node2D targetsNode = targetPage.GetNode<Node2D>("Targets");
        for (int i = 0; i < 4; i++)
        {
            String s = "Target" + (i + 1);
            targets.Add(targetPage.GetNode<Node2D>("Targets").GetNode<StaticBody2D>(s));
        }
        numberOfTargets = 4;
        rbl = battlePage.GetNode<RichTextLabel>("RichTextLabel");
        timer = targetPage.GetNode<Timer>("GameTImer");
        timer.Connect("timeout", this, "onTimeout");
        timer.WaitTime = 10;
        startingPos = player.Position;
    }

    public void minigameStart()
    {
        targetPage.Visible = true;
        battlePage.Visible = false;
        foreach (StaticBody2D target in targets)
        {
            target.Visible = true;
        }
        numberOfTargets = 4;
        minigamePlaying = true;
        player.Position = startingPos;
        timer.Start();
    }

    public void onTimeout()
    {
        minigamePlaying = false;
        rbl.Text += "You casted normally\n";
        targetPage.Visible = false;
        battlePage.Visible = true;
    }

    public void _on_Target_visibility_changed(){
        if (minigamePlaying)
        {
            numberOfTargets--;
            if (numberOfTargets == 0)
            {
                GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
                rbl.Text += "You cast the spell perfectly and absorb some of the energy back!\n";
                gp.currentPoints += 2;
                gp.perfectSpell = true;
                targetPage.Visible = false;
                battlePage.Visible = true;
                minigamePlaying = false;
                timer.Stop();
            }
        }
    }

}
