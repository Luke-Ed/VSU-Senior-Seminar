using Godot;
using System;
using System.Collections.Generic;

public class Simon : Node
{
    public Label orderLabel;
    public Timer timer;
    public List<int> combination = new List<int>();
    public int count = 0;
    public List<Button> buttons = new List<Button>();
    public List<int> userAnswer = new List<int>();
    public override void _Ready()
    {
        var random = new Random();
        orderLabel = GetNode<Label>("Order");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "onTimeout");
        timer.WaitTime = 3;
        for (int i = 0; i < 4; i++)
        {
            string tempButtonName = "Button" + (i + 1).ToString();
            Button tempButton = GetNode<Button>(tempButtonName);
            buttons.Add(tempButton);
            tempButton.Disabled = true;
           int temp = random.Next(1, 5);
            combination.Add(temp);   
        }
        timer.Start();
    }

    public void onTimeout()
    {
        orderLabel.Text = "";
        if (count <= 3)
        {
            showColor();
        }
        else
        {
            foreach (Button b in buttons)
            {
                b.Disabled = false;
            }
        }
    }

    public void showColor()
    {
        int number = combination[count];
        string orderShown = (count + 1).ToString() + ". " + number.ToString();
        orderLabel.Text = orderShown;
        count++;
        timer.Start();
    }

    public void checkAnswer()
    {
        Boolean isCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (userAnswer[i] != combination[i])
            {
                isCorrect = false;
            }
        }
        if (isCorrect)
        {
            orderLabel.Text = "Correct!";
        }
        else
        {
            orderLabel.Text = "Incorrect!";
        }
        timer.Stop();
    }

    public void _on_Button1_pressed()
    {
        userAnswer.Add(1);
        if(userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button2_pressed()
    {
        userAnswer.Add(2);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button3_pressed()
    {
        userAnswer.Add(3);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button4_pressed()
    {
        userAnswer.Add(4);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }


}
