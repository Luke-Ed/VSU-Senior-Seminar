using Godot;
using System;

public class TurnQueue : Node
{
    Node currentTurn;
    Godot.Collections.Array combatants;
    public override void _Ready()
    {
        combatants = getCombatants();
        printQueue();
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
}
