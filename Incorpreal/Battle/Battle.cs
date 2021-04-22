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
    public ColorRect battlePage;
    public Simon s;
    public Godot.Timer timer;
    public HitTheTarget_Engan HitTheTarget;
    private TimingGame _timingGame;

    public Battle()
    {

    }

    public override void _Ready()
    {
        s = (Simon)GetNode("SimonGame");
        HitTheTarget = (HitTheTarget_Engan)GetNode("HitTheTarget_Engan");
        battlePage = GetNode<ColorRect>("BattlePage");
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        tq = (TurnQueue)GetNode("/root/Tq");
        tq.combatants = tq.getCombatants();
        tq.setStats();
        player = (Node)tq.combatants[0] as KinematicBody2D;
        enemy = (Node)tq.combatants[1] as KinematicBody2D;
        currentFighterPos = 0;
        playerHP = battlePage.GetNode<Label>("HealthLabel") as Label;
        enemyHP = battlePage.GetNode<Label>("EnemyHealth") as Label;
        rtl = battlePage.GetNode<RichTextLabel>("RichTextLabel") as RichTextLabel;
        rtl.Text = "You have encountered a(n): " + tq.enemyName + "\n";
        attackbtn = battlePage.GetNode<Button>("Attackbtn") as Button;
        spellbtn = battlePage.GetNode<Button>("Spellbtn") as Button;
        defendbtn = battlePage.GetNode<Button>("Defendbtn") as Button;
        timer = battlePage.GetNode<Godot.Timer>("Timer") as Godot.Timer;
        timer.WaitTime = 20;
        timer.Connect("timeout", this, "onTimeout");
        gp.hplabel = playerHP;
        gp.updateHealthLabel(playerHP);
        updateEnemyHealth();
        _timingGame = (TimingGame)GetNode("TimingGame_Engan");
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
                gp.didBlock = false;
                playerActed = false;
                if (gp.status != null)
                {
                    rtl.Text += "You are " + gp.status + "\n";
                    switch (gp.status)
                    {
                        case ("Bleeding"):
                            gp.CurrentHealth -= 2;
                            break;
                        default:
                            break;
                    }
                }
                gp.updateHealthLabel(playerHP);
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
                    if (gp.didBlock)
                    {
                        rtl.Text += "but you blocked perfectly!" + "\n";
                    }
                }
                else
                {
                    rtl.Text += "The attack passed through you" + "\n";
                }
                gp.updateHealthLabel(playerHP);
                updateEnemyHealth();
            }
            changeCurrentFighter();
            if (gp.CurrentHealth <= 0 || tq.enemyCurrentHP <= 0)
            {
                if (gp.CurrentHealth <= 0)
                {
                    rtl.Text += "You have lost the fight";
                    playerActed = false;
                    GetNode<ColorRect>("DeathScreen").Visible = true;
                    gp.status = null;
                }
                else
                {
                    rtl.Text += "You have won the fight";
                    gp.Experience += 10;
                    if(gp.Experience >= gp.ExperienceToNextLevel)
                    {
                        gp.status = null;
                        GetTree().ChangeScene("res://LevelUp.tscn");
                    }
                }
                fightOver = true;
            }
        }
        else
        {
            gp.status = null;
            GetTree().ChangeScene(gp.lastScene);
        }
    }

    private void displayPlayerOptions()
    {
        attackbtn.Visible = !attackbtn.Visible;
        spellbtn.Visible = !spellbtn.Visible;
        if (gp.currentPoints < 5)
        {
            spellbtn.Disabled = true;
        }
        else
        {
            spellbtn.Disabled = false;
        }
        defendbtn.Visible = !defendbtn.Visible;
    }

    public void changeCurrentFighter()
    {
        currentFighterPos = (currentFighterPos + 1) % tq.combatants.Count;
    }

    public override void _Input(InputEvent @event)
    {
        if (playerActed && battlePage.Visible)
        {
            if (@event.IsActionPressed("Continue"))
            {
                fight();
            }
        }
    }

    public void _on_Attackbtn_pressed()
    {
        timer.Stop();
        _timingGame.startMinigame();
        playerActed = true;
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void _on_Spellbtn_pressed()
    {
        timer.Stop();
        player.Call("castSpell");
        rtl.Text += "You cast a spell at the " + tq.enemyName + "\n";
        playerActed = true;
        HitTheTarget.minigameStart();
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void _on_Defendbtn_pressed()
    {
        timer.Stop();
        gp.isDefending = true;
        rtl.Text += "You enter a defending stance" + "\n";
        playerActed = true;
        s.startMinigame();
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void onTimeout()
    {
        displayPlayerOptions();
        rtl.Text += "You spent too long and the " + tq.enemyName + " attacks!\n";
        playerActed = true;
    }

    public void _on_Resetbtn_pressed()
    {
        //This should later be replaced with loading a save from Eli if possible.
        gp.playerCharacter = null;
        gp.enemyFought.Clear();
        gp._inventory.Clear();
        gp._equipedArmor = null;
        gp._equipedWeapon = null;
        GetTree().ChangeScene(gp.lastScene);
    }

    public void _on_Quitbtn_pressed()
    {
        GetTree().Quit();
    }
}
