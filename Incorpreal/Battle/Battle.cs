using Godot;
using System;
using System.Threading;

public class Battle : Node
{
    public Label enemyHP;
    public TurnQueue tq;
    public GlobalPlayer gp;
    public int currentFighterPos;
    public Label playerHP;
    public Node player;
    public Node enemy;
    public RichTextLabel rtl;
    public Boolean fightOver = false;
    public Button attackbtn, spellbtn, defendbtn;
    public Boolean playerActed = true;
    public Godot.Timer timer;

    public Battle()
    {

    }

    public override void _Ready()
    {
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        tq = (TurnQueue)GetNode("/root/Tq");
        tq.combatants = tq.getCombatants();
        tq.setStats();
        player = (Node)tq.combatants[0] as KinematicBody2D;
        enemy = (Node)tq.combatants[1] as KinematicBody2D;
        currentFighterPos = 0;
        playerHP = GetNode<Label>("HealthLabel") as Label;
        enemyHP = GetNode<Label>("EnemyHealth") as Label;
        rtl = GetNode<RichTextLabel>("RichTextLabel") as RichTextLabel;
        rtl.Text = "You have encountered a(n): " + tq.enemyName + "\n";
        attackbtn = GetNode<Button>("Attackbtn") as Button;
        spellbtn = GetNode<Button>("Spellbtn") as Button;
        defendbtn = GetNode<Button>("Defendbtn") as Button;
        timer = GetNode<Godot.Timer>("Timer") as Godot.Timer;
        gp.updateHealthLabel(playerHP);
        updateEnemyHealth();
    }

    public void _on_TakeDamageButton_pressed()
    {
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        gp.takeDamage(5);
        var healthLabel = GetNode<Label>("HealthLabel") as Label;
        gp.updateHealthLabel(healthLabel);
    }

    public void updateEnemyHealth()
    {
        String text = "Enemy Health: " + tq.enemyCurrentHP + "/" + tq.enemyMaxHP;
        if (enemyHP != null)
        {
            enemyHP.Text = text;
        }
    }

    public void fight()
    {
        if (!fightOver)
        {
            if (currentFighterPos == 0)
            {
                gp.isDefending = false;
                playerActed = false;
                displayPlayerOptions();
                rtl.Text += "Choose an action \n";
                timer.Start();
            }
            else
            {
                Boolean didHit = (bool)enemy.Call("playTurn");
                if (didHit)
                {
                    rtl.Text += "You got hit by the " + tq.enemyName + "\n";
                }
                else
                {
                    rtl.Text += "The attack passed through you" + "\n";
                }
                gp.updateHealthLabel(playerHP);
            }
            changeCurrentFighter();
            if (gp.CurrentHealth <= 0 || tq.enemyCurrentHP <= 0)
            {
                if (gp.CurrentHealth <= 0)
                {
                    rtl.Text += "You have lost the fight";
                    playerActed = false;
                    GetNode<ColorRect>("DeathScreen").Visible = true;
                }
                else
                {
                    rtl.Text += "You have won the fight";
                    gp.Experience += 10;
                    if(gp.Experience >= gp.ExperienceToNextLevel)
                    {
                        GetTree().ChangeScene("res://LevelUp.tscn");
                    }
                }
                fightOver = true;
            }
        }
        else
        {
            GetTree().ChangeScene(gp.lastScene);
        }
    }

    private void displayPlayerOptions()
    {
        attackbtn.Visible = !attackbtn.Visible;
        spellbtn.Visible = !spellbtn.Visible;
        defendbtn.Visible = !defendbtn.Visible;
    }

    public void changeCurrentFighter()
    {
        currentFighterPos = (currentFighterPos + 1) % tq.combatants.Count;
    }

    public override void _Input(InputEvent @event)
    {
        if (playerActed)
        {
            if (@event.IsActionPressed("Continue"))
            {
                fight();
            }
        }
    }

    public void _on_Attackbtn_pressed()
    {
        float timeLeft = timer.TimeLeft;
        if (timeLeft > 0)
        {
            Boolean didHit = (bool)player.Call("attackEnemy");
            if (didHit)
            {
                rtl.Text += "You hit the " + tq.enemyName + "\n";
            }
            else
            {
                rtl.Text += "You missed the " + tq.enemyName + "\n";
            }
        }
        else
        {
            rtl.Text += "You heitated too long and the " + tq.enemyName + " attacks!";
        }
        playerActed = true;
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void _on_Spellbtn_pressed()
    {
        if (gp.currentPoints >= 5)
        {
            float timeLeft = timer.TimeLeft;
            if (timeLeft > 0)
            {
                player.Call("castSpell");
                rtl.Text += "You cast a spell at the " + tq.enemyName + "\n";
            }
            playerActed = true;
            updateEnemyHealth();
            displayPlayerOptions();
        }
        else
        {
            rtl.Text += "You heitated too long and the " + tq.enemyName + " attacks!";
        }
    }

    public void _on_Defendbtn_pressed()
    {
        float timeLeft = timer.TimeLeft;
        if (timeLeft > 0)
        {
            gp.isDefending = true;
            rtl.Text += "You enter a defending stance" + "\n";
        }
        else
        {
            rtl.Text += "You heitated too long and the " + tq.enemyName + " attacks!";
        }
        playerActed = true;
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void _on_Resetbtn_pressed()
    {
        gp.playerCharacter = null;
        gp.playerLocation = new Vector2(324, 179);
        gp.nodePaths.Clear();
        gp.createPlayer();
        GetTree().ChangeScene(gp.lastScene);
    }

    public void _on_Quitbtn_pressed()
    {
        GetTree().Quit();
    }
}
