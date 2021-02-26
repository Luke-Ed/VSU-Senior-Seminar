using Godot;
using System;

public class TurnQueue : Node
{
    Node currentTurn;
    public Godot.Collections.Array combatants;
    public int enemyMaxHP;
    public int enemyCurrentHP;
    public int enemyAttack;
    public string enemyName;
    public Node EnemyNode;
    public override void _Ready()
    {

    }


    public Godot.Collections.Array getCombatants()
    {
        return GetChildren();
    }

    public void printQueue()
    {
        string s = "";
        for (int i = 0; i < combatants.Count; i++)
        {
            s += GetChild(i).Name + " ";
        }
        Console.WriteLine(s);
    }

    public void removeChildren()
    {

        for (int i = 0; i < GetChildCount(); i++)
        {
            Node n = this.GetChild(i);
            RemoveChild(n);
            n.QueueFree();
        }
    }

    public void setStats()
    {
        Node enemy = (Node)combatants[1];
        EnemyNode = enemy;
        if (enemy.Get("health") != null && enemy.Get("currentHealth") != null)
        {
            enemyMaxHP = (int)enemy.Get("health");
            enemyCurrentHP = (int)enemy.Get("currentHealth");
            enemyName = (string)enemy.Get("name");
            enemyAttack = (int)enemy.Get("attack");
        }
    }
}
