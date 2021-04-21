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
    public List<String> enemyFought;
    public Label hplabel;
    public string lastScene;
    public Boolean isDefending = false;
    public Boolean didBlock = false;
    public Boolean perfectSpell = false;
    public Boolean isPossesing = false;
    public String status;
    public List<Item> _inventory { get; set; }
    public Item _equipedWeapon { get; set; }
    public Item _equipedArmor { get; set; }
    public Boolean _goodHit { get; set; }
    public Boolean _perfectHit { get; set; }

    public void updateHealthLabel(Label l)
    {
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
        enemyFought = new List<String>();
        spiritPoints = 5 + Intelligence;
        currentPoints = spiritPoints;
        baseStat = 5;
        _inventory = new List<Item>();
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
                if (_goodHit)
                {
                    if (_perfectHit)
                    {
                        tq.enemyCurrentHP -= Convert.ToInt32(Math.Floor(AttackDamage * 2 * 1.5));
                        return true;
                    }
                    tq.enemyCurrentHP -= Convert.ToInt32(Math.Floor(AttackDamage * 2 * 1.25));
                    return true;
                }
                tq.enemyCurrentHP -= AttackDamage * 2;
                return true;
            }
            if (_goodHit)
            {
                if (_perfectHit)
                {
                    tq.enemyCurrentHP -= Convert.ToInt32(Math.Floor(AttackDamage * 1.5));
                    return true;
                }
                tq.enemyCurrentHP -= Convert.ToInt32(Math.Floor(AttackDamage * 1.25));
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
        if (Stat == 1)
        {
            Strength++;
        }
        else if (Stat == 2)
        {
            Dexterity++;
        }
        else if (Stat == 3)
        {
            Vitality++;
        }
        else if (Stat == 4)
        {
            Intelligence++;
        }
        else if (Stat == 5)
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

    public void updateHealth()
    {
        MaxHealth = baseStat + Vitality;
    }

}
