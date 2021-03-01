using Godot;
using System;

public class Battle : Node
{
    public Label enemyHP;
    public TextEdit txtbox;
    public Button btn;
    public TurnQueue tq;
    public GlobalPlayer gp;
    public int currentFighterPos;
    public Label playerHP;
    public Node player;
    public Node enemy;

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
        txtbox = GetNode<TextEdit>("TextEdit") as TextEdit;
        txtbox.Text = "You have encountered a(n): " + tq.enemyName + "\n";
        btn = GetNode<Button>("Button2") as Button;
        gp.updateHealthLabel(playerHP);
        updateEnemyHealth();
    }

    //This is just for demo expamples to not get stuck on battle screen.
    public void _on_Button_pressed()
    {
       GetTree().ChangeScene(gp.lastScene);
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

    public void _on_Button2_pressed()
    {
        if (currentFighterPos == 0)
        {
            Boolean didHit = (bool)player.Call("playTurn");
            if (didHit)
            {
                txtbox.Text += "You hit the " + tq.enemyName + "\n";
            }
            else
            {
                txtbox.Text += "You missed the " + tq.enemyName + "\n";
            }
            updateEnemyHealth();
            btn.Text = "Continue";
        }
        else
        {
            Boolean didHit = (bool)enemy.Call("playTurn");
            if (didHit)
            {
                txtbox.Text += "You got hit by the " + tq.enemyName + "\n";
            }
            else
            {
                txtbox.Text += "The attack passed through you" + "\n";
            }
            gp.updateHealthLabel(playerHP);
            btn.Text = "Attack";
        }
        changeCurrentFighter();
        if (gp.CurrentHealth <= 0 || tq.enemyCurrentHP <= 0)
        {
            if (gp.CurrentHealth <= 0)
            {
                txtbox.Text += "You have lost the fight";
            }
            else
            {
                txtbox.Text += "You have won the fight";
            }
            btn.Visible = false;
        }
    }

    public void changeCurrentFighter()
    {
        currentFighterPos = (currentFighterPos + 1) % tq.combatants.Count; 
    }
}
