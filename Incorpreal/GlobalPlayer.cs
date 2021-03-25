using Godot;
using System;
using System.Collections.Generic;

public class GlobalPlayer : Node {
  public Vector2 PlayerLocation;
  public NodePath enemyPath;
    public Player PlayerCharacter;
  public int Level, AttackDamage, ExperienceToNextLevel, baseStat, spiritPoints, currentPoints;
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
    String text = "Your Health: " + PlayerCharacter.CurrentHealth + "/" + PlayerCharacter.MaxHealth;
    text += "\n Spirit Points: " + currentPoints + "/" + spiritPoints;
    if (l != null) {
      l.Text = text;
    }
  }

  public void createPlayer() {
    Player temp = new Player("Melee");
    PlayerCharacter = temp;
    CharacterClass = PlayerCharacter.CharacterClass;
    AttackDamage = PlayerCharacter.AttackDamage;
    PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
    Level = PlayerCharacter.Level;
    ExperienceToNextLevel = PlayerCharacter.ExperienceToNextLevel;
    enemyFought = new List<String>();
    spiritPoints = 5 + PlayerCharacter.Intelligence;
    currentPoints = spiritPoints;
    baseStat = 5;
  }

    public Boolean takeDamage(int damage) {
      Random random = new Random();
      int roll = random.Next(101);
      if (roll >= 100 - PlayerCharacter.Intelligence) {
        return false;
      }
      else {
        if ((PlayerCharacter.CurrentHealth - damage) <= 0) {
        PlayerCharacter.CurrentHealth = 0;
        return true;
      }
      PlayerCharacter.CurrentHealth -= damage;
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
        if (roll >= 20 - PlayerCharacter.Intelligence) {
            //Checking for critical hit based on luck
            if (roll >= 100 - PlayerCharacter.Luck) {
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
        tq.EnemyCurrentHp -= PlayerCharacter.Intelligence + 5;
    }

  //When character levels up choose a stat to increase;
  public void LevelUp(int stat) {
    Level++;
    baseStat += 5;

    switch (stat) {
      case 0:
        PlayerCharacter.Strength++;
        break;
      case 1:
        PlayerCharacter.Dexterity++;
        break;
      case 2:
        PlayerCharacter.Vitality++;
        break;
      case 3:
        PlayerCharacter.Intelligence++;
        break;
      case 4:
        PlayerCharacter.Luck++;
        break;
    }

    if (CharacterClass == "Ranged") {
      AttackDamage += baseStat + PlayerCharacter.Dexterity;
    }
    else {
      AttackDamage += baseStat + PlayerCharacter.Strength;
    }
    PlayerCharacter.MaxHealth = baseStat + PlayerCharacter.Vitality;
    PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
    spiritPoints = baseStat + PlayerCharacter.Intelligence;
    currentPoints = spiritPoints;
    ExperienceToNextLevel += 10;
    }
}
