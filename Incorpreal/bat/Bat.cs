using Godot;
using System;
using System.Collections.Generic;

public class Bat : KinematicBody2D
{
    [Export]
    public int moveSpeed = 50;
    public int attack;
    public int health;
    public int currentHealth;
    public string enemyName;
    public GlobalPlayer gp;
    public KinematicBody2D player;
    private Navigation2D _navigation;
    private Vector2 _startingPos;
    private Timer timer;
    private Boolean _battleStarting;
    public Boolean _onCamera { get; set; }

    public override void _PhysicsProcess(float delta)
    {
        try
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            MoveAndCollide(direction * moveSpeed * delta);
        }
        catch
        {

        }
    }

    public override void _Ready()
    {
        _startingPos = this.Position;
        var filePath = "res://Enemies/enemies.txt";
        File newFile = new File();
        newFile.Open(filePath, File.ModeFlags.Read);
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        while (!newFile.EofReached())
        {
            String s = newFile.GetLine();
            if (Name.Contains(s)){
                enemyName = s;
                attack = int.Parse(newFile.GetLine());
                health = int.Parse(newFile.GetLine());
                currentHealth = health;
            }
        }
        if (HasNode("Timer")) //Making sure it has the timer node because the dummy enemy in turn queue does not.
        {
            timer = GetNode<Timer>("Timer");
            timer.Connect("timeout", this, "onTimeout");
            timer.WaitTime = 10;
        }
    }

    public Boolean playTurn()
    {
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        Boolean didHit = false;
        if (!gp.isDefending)
        {
            didHit = gp.takeDamage(attack);
        }
        else
        {
            if (gp.didBlock)
            {
                didHit = gp.takeDamage(0);
            }
            else
            {
                didHit = gp.takeDamage(attack / 2);
            }
        }
        if (didHit)
        {
            switch (enemyName)
            {
                case ("Bat"):
                    if (tq.enemyCurrentHP + attack <= tq.enemyMaxHP)
                    {
                        tq.enemyCurrentHP += attack;
                    }
                    break;
                case ("Bear"):
                    gp.status = "Bleeding";
                    break;
                case ("Goblin"):
                    //Need something for goblin
                    break;
                case ("Skeleton"):
                    //Need something for skeleton
                    break;
                case ("Snake"):
                    //Need something for snake
                    break;
                case ("Wolf"):
                    //Need something for wolf
                    break;
                case ("Necromancer"):
                    //Need something for big bad
                    break;
                default:
                    break;
            }
        }
        return didHit;
    }


    public void Hit()
    { 
        if (!gp.isPossesing) { //Prevents bat from attacking other (possessed) enemies. Should add this to other enemies code eventually
            timer.Stop();
            _battleStarting = true; //Setting boolean statement to true once the battle is starting.
            gp.enemyFought.Add(this.Name);
            TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
            tq.GetChild(1).Name = enemyName;
            tq.GetChild(1).Call("_Ready");
            GetTree().ChangeScene("res://Battle.tscn");
        }
    }

    //If player enters the range, sets player to the player body and adds the enemy to the group following which
    //makes it stop following the given path and start following the player using _PhysicsProcess method.
    public void _on_Area2D_body_entered(Node body)
    {
        if (body.Name == "Player")
        {
            player = (KinematicBody2D)body;
            this.AddToGroup("Following");
        }
    }


    //Will remove the body from player variable so it has nothing to follow and will start the timer (see below onTimeout()).
    public void _on_Area2D_body_exited(Node body)
    {
        if (body.Name == "Player")
        {
            player = null;
            if (!_battleStarting) //Checking if when the body is leaving if it is due ot the battle starting it will not start the timer.
            {
                timer.Start();
            }
        }
    }


    //After the player gets out of range of the enemy and will continue to stay in that spot after 10 seconds if the enemy is no longer on screen then it is returned to the original starting position
    //and follows the original path if it has one by removing it from the "Following" group. If the enemy is still on screen the timer refreshes and will check again in another 10 seconds.
    public void onTimeout()
    {
        if (_onCamera)
        {
            timer.Start();
        }
        
        else
        {
            this.Position = _startingPos;
            this.RemoveFromGroup("Following");
        }
    }
}