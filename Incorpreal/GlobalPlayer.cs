using Godot;
using System;
using System.Collections.Generic;

public class GlobalPlayer : Node
{
    public Vector2 playerLocation;
    public NodePath enemyPath;
    public Player playerCharacter;
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage;
    public String CharacterClass;
    public Node PC;
    public Node Enemy;
    public List<NodePath> nodePaths;

    public void updateHealthLabel(Label l)
    {
        String text = "Health: " + CurrentHealth + "/" + MaxHealth;
        if (l != null)
        {
            l.Text = text;
        }
    }

    public void createPlayer()
    {
        Player temp = new Player("Melee");
        this.playerCharacter = temp;
        Strength = playerCharacter.Strength;
        Dexterity = playerCharacter.Dexterity;
        Vitality = playerCharacter.Vitality;
        Intelligence = playerCharacter.Intelligence;
        Luck = playerCharacter.Luck;
        CharacterClass = playerCharacter.CharacterClass;
        AttackDamage = playerCharacter.AttackDamage;
        Experience = playerCharacter.Experience;
        MaxHealth = playerCharacter.MaxHealth;
        CurrentHealth = playerCharacter.CurrentHealth;
        Level = playerCharacter.Level;
        nodePaths = new List<NodePath>();
    }

    public void takeDamage(int damage)
    {
        Random random = new Random();
        int roll = random.Next(101);
        if ((CurrentHealth - damage) <= 0)
        {
            CurrentHealth = 0;
        }
        else
        {
            CurrentHealth -= damage;
        }
        /* Removing random chance to miss for purpose of demo presentation
        if (roll >= 100 - gp.Intelligence)
        {
            gp.CurrentHealth -= damage;
        }
        */
    }

    //Returns the amount of damage done if you are able to hit
    public int AttackEnemy()
    {
        Random random = new Random();
        int roll = random.Next(101);
        //Checking to make sure the ghost can hit the enemy based on intelligence
        if (roll >= 20 - Intelligence)
        {
            //Checking for critical hit based on luck
            if (roll >= 100 - Luck)
            {
                return AttackDamage * 2;
            }
            return AttackDamage;
        }
        else
        {
            return 0;
        }
    }

    //When character levels up choose a stat to increase;
    public void LevelUp(String Stat)
    {
        Level++;
        if (Stat == "Strength")
        {
            Strength++;
            if (CharacterClass == "Melee")
            {
                AttackDamage = 5 + Strength;
            }
        }
        else if (Stat == "Dexterity")
        {
            Dexterity++;
            if (CharacterClass == "Ranged")
            {
                AttackDamage = 5 + Dexterity;
            }
        }
        else if (Stat == "Vitality")
        {
            Vitality++;
            MaxHealth = 5 + Vitality;
        }
        else if (Stat == "Intelligence")
        {
            Intelligence++;
        }
        else if (Stat == "Luck")
        {
            Luck++;
        }
        CurrentHealth = MaxHealth;
    }



}
