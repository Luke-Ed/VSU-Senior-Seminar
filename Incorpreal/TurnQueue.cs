using Godot;
using System;

public class TurnQueue : Node {
  Node currentTurn;
  public Godot.Collections.Array Combatants;
  public int EnemyMaxHp { get; private set; }
  public int EnemyCurrentHp { get; set; }
  public string EnemyType { get; private set; }
  private int _enemyAttack;
  
  public Node EnemyNode;
  public override void _Ready() { }
  
  public Godot.Collections.Array GetCombatants() {
    return GetChildren();
  }

  public void PrintQueue() {
    string s = "";
    for (int i = 0; i < Combatants.Count; i++)
    {
      s +="Type/Name: " + GetChild(i).Name + " , Health: " + GetChild(i).Get("Health") +" .";
    }
    Console.WriteLine(s);
    }

    public void RemoveChildren() {
      for (int i = 0; i < GetChildCount(); i++) {
        Node n = this.GetChild(i);
        RemoveChild(n);
        n.QueueFree();
      }
    }

    public void SetStats() {
      Node enemy = (Node)Combatants[1];
      EnemyNode = enemy;
      if (enemy.Get("Health") == null || enemy.Get("CurrentHealth") == null) return;
      EnemyMaxHp = (int)enemy.Get("Health");
      EnemyCurrentHp = (int)enemy.Get("CurrentHealth");
      EnemyType = (string)enemy.Get("EnemyType");
      _enemyAttack = (int)enemy.Get("Attack");
    }
}
