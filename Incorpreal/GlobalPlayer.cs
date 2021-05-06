using System;
using System.Collections.Generic;
using Godot;

namespace Incorpreal {
  public class GlobalPlayer : Node {
    public Vector2 PlayerLocation;
    public NodePath enemyPath;
    public Player PlayerCharacter;
    
    public Node PC;
    public Node Enemy;
    public List<String> EnemiesFought;
    public Label hplabel;
    public string lastScene;
    public Boolean isDefending = false;
    public Boolean didBlock = false;
    public Boolean perfectSpell = false;
    public Boolean isPossesing = false;
    public List<Item> Inventory { get; set; }
    public Item EquippedWeapon { get; set; }
    public Item EquippedArmor { get; set; }
    public Boolean GoodHit { get; set; }
    public Boolean PerfectHit { get; set; }
    public int BaseStat { get; set; }
    public int NumOpenedChests {get; set; }
    public String EnemyPossessed { get; set; }
    public void updateHealthLabel(Label l) {
      String text = "Your Health: " + PlayerCharacter.CurrentHealth + "/" + PlayerCharacter.MaxHealth;
      text += "\n Spirit Points: " + PlayerCharacter.CurrentSpiritPoints + "/" + PlayerCharacter.MaxSpiritPoints;
      if (l != null) {
        l.Text = text;
      }
    }


    public void createPlayer() {
      Player temp = new Player();
      PlayerCharacter = temp;
      PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
      EnemiesFought = new List<String>();
      PlayerCharacter.MaxSpiritPoints = 5 + PlayerCharacter.Intelligence;
      PlayerCharacter.CurrentSpiritPoints = PlayerCharacter.MaxSpiritPoints;
      BaseStat = 5;
      Inventory = new List<Item>();
    }

    public Boolean takeDamage(int damage) {
      Random random = new Random();
      int roll = random.Next(101);
      if (roll >= (100 - PlayerCharacter.Intelligence)) {
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
    public Boolean AttackEnemy(){
      TurnQueue tq = (TurnQueue)GetNode("/root/Tq");
      Random random = new Random();
      int roll = random.Next(101);
      //Checking to make sure the ghost can hit the enemy based on intelligence
      if (roll >= 20 - PlayerCharacter.Intelligence) {
        //Checking for critical hit based on luck
        if (roll >= 100 - PlayerCharacter.Luck){
          if (GoodHit){
            if (PerfectHit){
              tq.EnemyCurrentHp -= Convert.ToInt32(Math.Floor(PlayerCharacter.AttackDamage * 2 * 1.5));
              return true;
            }
            tq.EnemyCurrentHp -= Convert.ToInt32(Math.Floor(PlayerCharacter.AttackDamage * 2 * 1.25));
            return true;
          }
          tq.EnemyCurrentHp -= PlayerCharacter.AttackDamage * 2;
          return true;
        }
        // If not Critical Hit:
        if (GoodHit){
          if (PerfectHit){
            tq.EnemyCurrentHp -= Convert.ToInt32(Math.Floor(PlayerCharacter.AttackDamage * 1.5));
            return true;
          }
          tq.EnemyCurrentHp -= Convert.ToInt32(Math.Floor(PlayerCharacter.AttackDamage * 1.25));
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
      PlayerCharacter.CurrentSpiritPoints -= 5;
      updateHealthLabel(hplabel);
      tq.EnemyCurrentHp -= PlayerCharacter.Intelligence + 5;
    }

    //When character levels up choose a stat to increase;
    public void LevelUp(int stat) {
      PlayerCharacter.Level++;
      BaseStat += 5;

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


      if (EquippedWeapon == null || EquippedWeapon.Stat == "Strength"){
        PlayerCharacter.AttackDamage += BaseStat + PlayerCharacter.Strength;
      }
      else {
        PlayerCharacter.AttackDamage += BaseStat + PlayerCharacter.Dexterity;
      }

      PlayerCharacter.MaxHealth = BaseStat + PlayerCharacter.Vitality;
      PlayerCharacter.CurrentHealth = PlayerCharacter.MaxHealth;
      PlayerCharacter.MaxSpiritPoints = BaseStat + PlayerCharacter.Intelligence;
      PlayerCharacter.CurrentSpiritPoints = PlayerCharacter.MaxSpiritPoints;
      PlayerCharacter.ExperienceToNextLevel += PlayerCharacter.Level * 10;
      PlayerCharacter.StatusEffect = String.Empty;
    }

    //public void updateHealth(){
    //  MaxHealth = baseStat + Vitality;
    //}
  }
}
