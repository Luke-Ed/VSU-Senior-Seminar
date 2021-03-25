using Godot;
using System;
using System.Collections.Generic;

public class GlobalPlayer : Node {
  public Vector2 PlayerLocation;
  public NodePath enemyPath;
  public Player PlayerCharacter;
  private int _baseStat, _spiritPoints;
  public int CurrentPoints;
  // Note / Todo: Move Spirit points into player.cs, since all other values are stored there.
  public String CharacterClass;
  public Node PC;
  public Node Enemy;
  public List<String> enemiesFought;
  public Label hplabel;
  public string lastScene;
  public Boolean isDefending = false;
  public Boolean didBlock = false;
  public Boolean perfectSpell = false;
  public Boolean isPossesing = false;
  public String Status {get; set;}
  public void updateHealthLabel(Label l) {
    String text = "Your Health: " + PlayerCharacter.CurrentHealth + "/" + PlayerCharacter.MaxHealth;
    text += "\n Spirit Points: " + CurrentPoints + "/" + _spiritPoints;
    if (l != null) {
      l.Text = text;
    }
  }

  public void createPlayer() {
    Player temp = new Player("Melee");
    PlayerCharacter = temp;
    CharacterClass = PlayerCharacter.CharacterPlayerClass;
    PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
    enemiesFought = new List<String>();
    _spiritPoints = 5 + PlayerCharacter.Intelligence;
    CurrentPoints = _spiritPoints;
    _baseStat = 5;
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
                tq.EnemyCurrentHp -= PlayerCharacter.AttackDamage * 2;
                return true;
            }
            tq.EnemyCurrentHp -= PlayerCharacter.AttackDamage;
            return true;
        }
        else {
            tq.EnemyCurrentHp -= 0;
            return false;
        }
    }

    public void castSpell () {
        TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
        CurrentPoints -= 5;
        updateHealthLabel(hplabel);
        tq.EnemyCurrentHp -= PlayerCharacter.Intelligence + 5;
    }

  //When character levels up choose a stat to increase;
  public void LevelUp(int stat) {
    PlayerCharacter.Level++;
    _baseStat += 5;

    switch (stat) {
      case 1:
        PlayerCharacter.Strength++;
        break;
      case 2:
        PlayerCharacter.Dexterity++;
        break;
      case 3:
        PlayerCharacter.Vitality++;
        break;
      case 4:
        PlayerCharacter.Intelligence++;
        break;
      case 5:
        PlayerCharacter.Luck++;
        break;
    }

    if (CharacterClass == "Ranged") {
      PlayerCharacter.AttackDamage += _baseStat + PlayerCharacter.Dexterity;
    }
    else {
      PlayerCharacter.AttackDamage += _baseStat + PlayerCharacter.Strength;
    }
    PlayerCharacter.MaxHealth = _baseStat + PlayerCharacter.Vitality;
    PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
    _spiritPoints = _baseStat + PlayerCharacter.Intelligence;
    CurrentPoints = _spiritPoints;
    PlayerCharacter.ExperienceToNextLevel += 10;
    }
}
