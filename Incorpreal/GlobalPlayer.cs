using Godot;
using System;
using System.Collections.Generic;

public class GlobalPlayer : Node
{
    public Vector2 playerLocation;
    public NodePath enemyPath;
    public Player playerCharacter;
    public int Strength, Dexterity, Vitality, Intelligence, Luck, Experience, MaxHealth, CurrentHealth, Level, AttackDamage, ExperienceToNextLevel, baseStat, spiritPoints, currentPoints;
    public String CharacterClass;
    public Node PC;
    public Node Enemy;
    public List<NodePath> nodePaths;
    public Label hplabel;
    public string lastScene;
    public Boolean isDefending = false;
    public Boolean didBlock = false;

    public void updateHealthLabel(Label l)
    {
        hplabel = l;
        String text = "Your Health: " + CurrentHealth + "/" + MaxHealth;
        text += "\n Spirit Points: " + currentPoints + "/" + spiritPoints;
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
        CurrentHealth = MaxHealth;
        Level = playerCharacter.Level;
        ExperienceToNextLevel = playerCharacter.ExperienceToNextLevel;
        nodePaths = new List<NodePath>();
        spiritPoints = 5 + Intelligence;
        currentPoints = spiritPoints;
        baseStat = 5;
    }

    public Boolean takeDamage(int damage)
    {
        Random random = new Random();
        int roll = random.Next(101);
        if (roll >= 100 - Intelligence)
        {
            return false;
        }
        else
        {
            if ((CurrentHealth - damage) <= 0)
            {
                CurrentHealth = 0;
                return true;
            }
            CurrentHealth -= damage;
            return true;
        }
    }

    //Returns the amount of damage done if you are able to hit
    public Boolean AttackEnemy()
    {
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        Random random = new Random();
        int roll = random.Next(101);
        //Checking to make sure the ghost can hit the enemy based on intelligence
        if (roll >= 20 - Intelligence)
        {
            //Checking for critical hit based on luck
            if (roll >= 100 - Luck)
            {
                tq.enemyCurrentHP -= AttackDamage * 2;
                return true;
            }
            tq.enemyCurrentHP -= AttackDamage;
            return true;
        }
        else
        {
            tq.enemyCurrentHP -= 0;
            return false;
        }
    }

    public void castSpell ()
    {
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        currentPoints -= 5;
        updateHealthLabel(hplabel);
        tq.enemyCurrentHP -= Intelligence + 5;
    }

    //When character levels up choose a stat to increase;
    public void LevelUp(int Stat)
    {
        Level++;
        baseStat += 5;
        if (Stat == 0)
        {
            Strength++;
        }
        else if (Stat == 1)
        {
            Dexterity++;
        }
        else if (Stat == 2)
        {
            Vitality++;
        }
        else if (Stat == 3)
        {
            Intelligence++;
        }
        else if (Stat == 4)
        {
            Luck++;
        }
        if (CharacterClass == "Ranged")
        {
            AttackDamage += baseStat + Dexterity;
        }
        else
        {
            AttackDamage += baseStat + Strength;
        }
        MaxHealth = baseStat + Vitality;
        CurrentHealth = MaxHealth;
        spiritPoints = baseStat + Intelligence;
        currentPoints = spiritPoints;
        ExperienceToNextLevel += 10;
    }
}
