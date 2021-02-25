using Godot;
using System;

public class TurnQueue : Node
{
    Node currentTurn;
    public Godot.Collections.Array combatants;
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
}
