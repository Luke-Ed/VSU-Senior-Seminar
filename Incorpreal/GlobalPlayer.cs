using Godot;
using System;
using System.Collections.Generic;

public class GlobalPlayer : Node {
    public Vector2 PlayerLocation;
    public NodePath enemyPath;
    public Player PlayerCharacter;
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

    public void updateHealthLabel(Label l) {
        String text = "Your Health: " + CurrentHealth + "/" + MaxHealth;
        text += "\n Spirit Points: " + currentPoints + "/" + spiritPoints;
        if (l != null) {
            l.Text = text;
        }
    }

    public void createPlayer() {
        Player temp = new Player("Melee");
        PlayerCharacter = temp;
        Strength = PlayerCharacter.Strength;
        Dexterity = PlayerCharacter.Dexterity;
        Vitality = PlayerCharacter.Vitality;
        Intelligence = PlayerCharacter.Intelligence;
        Luck = PlayerCharacter.Luck;
        CharacterClass = PlayerCharacter.CharacterClass;
        AttackDamage = PlayerCharacter.AttackDamage;
        Experience = PlayerCharacter.Experience;
        MaxHealth = PlayerCharacter.MaxHealth;
        CurrentHealth = MaxHealth;
        Level = PlayerCharacter.Level;
        ExperienceToNextLevel = PlayerCharacter.ExperienceToNextLevel;
        enemyFought = new List<String>();
        spiritPoints = 5 + Intelligence;
        currentPoints = spiritPoints;
        baseStat = 5;
    }

    public Boolean takeDamage(int damage)
    {
        Random random = new Random();
        int roll = random.Next(101);
        if (roll >= 100 - Intelligence) {
            return false;
        }
        else {
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
        if (roll >= 20 - Intelligence) {
            //Checking for critical hit based on luck
            if (roll >= 100 - Luck) {
                tq.EnemyCurrentHp -= AttackDamage * 2;
                return true;
            }
            tq.EnemyCurrentHp -= AttackDamage;
            return true;
        }
        else {
            tq.EnemyCurrentHp -= 0;
            return false;
        }
    }

    public void castSpell () {
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        currentPoints -= 5;
        updateHealthLabel(hplabel);
        tq.EnemyCurrentHp -= Intelligence + 5;
    }

  //When character levels up choose a stat to increase;
  public void LevelUp(int stat) {
    Level++;
    baseStat += 5;

    switch (stat) {
      case 0:
        Strength++;
        break;
      case 1:
        Dexterity++;
        break;
      case 2:
        Vitality++;
        break;
      case 3:
        Intelligence++;
        break;
      case 4:
        Luck++;
        break;
    }

    if (CharacterClass == "Ranged") {
      AttackDamage += baseStat + Dexterity;
    }
    else {
      AttackDamage += baseStat + Strength;
    }
    MaxHealth = baseStat + Vitality;
    CurrentHealth = MaxHealth;
    spiritPoints = baseStat + Intelligence;
    currentPoints = spiritPoints;
    ExperienceToNextLevel += 10;
    }
}
