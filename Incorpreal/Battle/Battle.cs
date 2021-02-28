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

    public Battle()
    {

    }

    public override void _Ready()
    {
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
        tq = (TurnQueue)GetNode("/root/Tq");
        tq.combatants = tq.getCombatants();
        tq.setStats();
        currentFighterPos = 0;
        playerHP = GetNode<Label>("HealthLabel") as Label;
        enemyHP = GetNode<Label>("EnemyHealth") as Label;
        txtbox = GetNode<TextEdit>("TextEdit") as TextEdit;
        txtbox.Text = "You have encountered a(n): " + tq.enemyName;
        btn = GetNode<Button>("Button2") as Button;
        gp.updateHealthLabel(playerHP);
        updateEnemyHealth();
    }

    //This is just for demo expamples to not get stuck on battle screen.
    public void _on_Button_pressed()
    {
       GetTree().ChangeScene("res://Game.tscn");
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
            int damage = gp.AttackEnemy();
            if (damage == gp.AttackDamage)
            {
                txtbox.Text = "You hit the " + tq.enemyName;
            }
            else if (damage == 0)
            {
                txtbox.Text = "You missed the " + tq.enemyName;
            }
            else
            {
                txtbox.Text = "You hit the " + tq.enemyName + " For massive Damage!";
            }
            tq.enemyCurrentHP -= damage;
            updateEnemyHealth();
            btn.Text = "Continue";
        }
        else
        {
            int oldDamage = gp.CurrentHealth;
            gp.takeDamage(tq.enemyAttack);
            gp.updateHealthLabel(playerHP);
            if (gp.CurrentHealth < oldDamage)
            {
                txtbox.Text = "You got hit by the " + tq.enemyName;
            }
            else
            {
                txtbox.Text = "The attack passed through you";
            }
            btn.Text = "Attack";
        }
        changeCurrentFighter();
        if (gp.CurrentHealth <= 0 || tq.enemyCurrentHP <= 0)
        {
            GetTree().ChangeScene("res://Game.tscn");
        }
    }

    public void changeButtonTxt(String s)
    {
        btn.Text = s;
    }

    public void changeCurrentFighter()
    {
        currentFighterPos = (currentFighterPos + 1) % tq.combatants.Count; 
    }
}
