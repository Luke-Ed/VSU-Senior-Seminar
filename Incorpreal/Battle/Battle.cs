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
    public ColorRect SimonPage;
    public Simon s;

    public Battle()
    {

    }

    public override void _Ready()
    {
        s = (Simon)GetNode("SimonGame");
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
        gp.updateHealthLabel(playerHP);
        updateEnemyHealth();
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
                displayPlayerOptions();
                rtl.Text += "Choose an action \n";
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
        Boolean didHit = (bool)player.Call("attackEnemy");
        if (didHit)
        {
            rtl.Text += "You hit the " + tq.enemyName + "\n";
        }
        else
        {
            rtl.Text += "You missed the " + tq.enemyName + "\n";
        }
        playerActed = true;
        updateEnemyHealth();
        displayPlayerOptions();
    }

    public void _on_Spellbtn_pressed()
    {
        if (gp.currentPoints >= 5)
        {
                player.Call("castSpell");
                rtl.Text += "You cast a spell at the " + tq.enemyName + "\n";
        }
            playerActed = true;
            updateEnemyHealth();
            displayPlayerOptions();
    }

    public void _on_Defendbtn_pressed()
    {
        gp.isDefending = true;
        rtl.Text += "You enter a defending stance" + "\n";
        playerActed = true;
        s.startMinigame();
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
